using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;

using System.Data.Odbc;
using System.Data.OleDb;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Text.RegularExpressions;
using Framework.Common.Logging;
using Framework.Common;

namespace Framework.DataAccess
{
    public class DataService
    {
        public EnumEnvironment Environment { get; set; }
        public string ConnectionString { get; private set; }
        private readonly ILogger _logger;
        private const int CommandTimeOut = 290;

        private SqlConnection _persistConnection;

        /// <summary>
        /// CTOR
        /// </summary>
        /// <param name="logger">The Logwriter used for logging the database calls.</param>
        /// <param name="connectionString"></param>
        public DataService(ILogger logger)
        {
            Init();
        }

        public DataService(LogWriter logger)
        {
            Init();
        }

        public DataService(string connectionString, LogWriter logger)
        {
            ConnectionString = connectionString;
        }

        public DataService(string connectionString, ILogger logger)
        {
            ConnectionString = connectionString;
            _logger = logger;
        }

        private void Init()
        {
            var _environment = string.Empty;
            try
            {
                _environment = ConfigurationManager.AppSettings["Environment"];
                if (string.IsNullOrEmpty(_environment) || _environment == "Local" || _environment == "Development" || _environment == "Stage") { _environment = "Development"; }
            }
            catch
            {
                _environment = "Development";
            }
            Environment = (EnumEnvironment)Enum.Parse(typeof(EnumEnvironment), _environment);
            ConnectionString = ConfigurationManager.ConnectionStrings[_environment].ConnectionString;
        }

        public SqlConnection GetSqlConnectionInstance()
        {
            return new SqlConnection(ConnectionString);
        }

        /// <summary>
        /// Executes non query Methods (DML operations; update, insert, delete)
        /// </summary>
        /// <param name="commandName">Name of the Stored Procedure</param>
        /// <param name="setParameters">Action Lambda </param>
        public void ExecuteNonQuery(string commandName, Action<SqlParameterCollection> setParameters = null)
        {
            using (SqlConnection connection = this.GetSqlConnectionInstance())
            {
                connection.Open();

                SqlCommand Command = new SqlCommand(commandName, connection);
                Command.CommandType = CommandType.StoredProcedure;
                Command.CommandTimeout = CommandTimeOut;
                if (setParameters != null) setParameters(Command.Parameters);

                Command.ExecuteNonQuery();
            }
        }

        public string ExecuteQueryTransaction(Action<SqlCommand> runActions)
        {
            using (SqlConnection connection = this.GetSqlConnectionInstance())
            {
                connection.Open();

                SqlCommand command = connection.CreateCommand();
                command.CommandTimeout = CommandTimeOut;
                command.CommandType = CommandType.StoredProcedure;
                SqlTransaction transaction;
                string result = "";

                // Start a local transaction.
                transaction = connection.BeginTransaction("Transaction");

                // Must assign both transaction object and connection
                // to Command object for a pending local transaction
                command.Connection = connection;
                command.Transaction = transaction;

                try
                {
                    runActions(command);
                    transaction.Commit();
                }
                catch (Exception ex)
                {
                    result = "Rollback: " + ex.Message;
                    try
                    {
                        transaction.Rollback();
                    }
                    catch (Exception ex2)
                    {
                        // This catch block will handle any errors that may have occurred
                        // on the server that would cause the rollback to fail, such as
                        // a closed connection.                    
                        result += "   RollBackError:" + ex2.Message;
                    }
                }
                return result;
            }
        }

        /// <summary>
        /// Executes non query methods using parameterized query. 
        /// </summary>
        /// <param name="sql">Sql text. Ensure sql string is parameterized</param>
        /// <param name="setParameters">sql parameter collection, lambda function</param>
        public void ExecuteNonQueryString(string sql, Action<SqlParameterCollection> setParameters = null)
        {
            using (SqlConnection connection = this.GetSqlConnectionInstance())
            {
                connection.Open();

                SqlCommand Command = new SqlCommand(sql, connection);
                Command.CommandType = CommandType.Text;
                Command.CommandTimeout = CommandTimeOut;
                if (setParameters != null) setParameters(Command.Parameters);

                Command.ExecuteNonQuery();
            }
        }



        public void OpenPersistantConnection()
        {

            if (_persistConnection == null)
            {
                //Get a new connection.
                _persistConnection = this.GetSqlConnectionInstance();
            }

            if (_persistConnection.State != ConnectionState.Closed)
            {
                ClosePersistantConnection();
            }
            _persistConnection.Open();
        }

        public void ClosePersistantConnection()
        {

            if (_persistConnection != null)
            {
                if (_persistConnection.State != ConnectionState.Closed)
                {
                    _persistConnection.Close();
                    _persistConnection.Dispose();
                }
            }
        }

        // Parameterized SQL String i.e. "SELECT field FROM table WHERE field = @argument"  ==> c.Add("@field", SqlDbType.VarChar).Value = xyz;
        public DataSet ExecuteParameterizedQueryUsingPersistConn(string queryString, Action<SqlParameterCollection> setParameters = null)
        {
            DataSet ds = new DataSet();

            SqlCommand cmd = new SqlCommand(queryString, _persistConnection);
            cmd.CommandTimeout = CommandTimeOut;
            cmd.CommandType = CommandType.Text;
            if (setParameters != null) setParameters(cmd.Parameters);
            SqlDataReader reader = cmd.ExecuteReader();

            var results = new DataTable();
            results.Load(reader);
            ds.Tables.Add(results);
            reader.Close();

            return ds;
        }

        public void ExecuteNonQueryStringUsingPersistConn(string sql, Action<SqlParameterCollection> setParameters = null)
        {
            SqlCommand Command = new SqlCommand(sql, _persistConnection);
            Command.CommandType = CommandType.Text;
            Command.CommandTimeout = CommandTimeOut;
            if (setParameters != null) setParameters(Command.Parameters);

            Command.ExecuteNonQuery();
        }






        // Parameterized SQL String i.e. "SELECT field FROM table WHERE field = @argument"  ==> c.Add("@field", SqlDbType.VarChar).Value = xyz;
        public DataSet ExecuteParameterizedQuery(string queryString, Action<SqlParameterCollection> setParameters = null)
        {
            DataSet ds = new DataSet();
            using (SqlConnection conn = this.GetSqlConnectionInstance())
            {
                SqlCommand cmd = new SqlCommand(queryString, conn);
                cmd.CommandType = CommandType.Text;
                cmd.CommandTimeout = CommandTimeOut;
                if (setParameters != null) setParameters(cmd.Parameters);

                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader(CommandBehavior.CloseConnection);

                var results = new DataTable();
                results.Load(reader);
                ds.Tables.Add(results);
                reader.Close();
            }
            return ds;
        }



        // Execute text query and return a scalar value of type T
        public T ExecuteScaler<T>(string queryString, Action<SqlParameterCollection> setParameters = null)
        {
            object result;
            using (SqlConnection connection = this.GetSqlConnectionInstance())
            {
                connection.Open();

                SqlCommand Command = new SqlCommand(queryString, connection);
                Command.CommandType = CommandType.Text;
                Command.CommandTimeout = CommandTimeOut;
                setParameters(Command.Parameters);

                result = Command.ExecuteScalar();
            }

            return (T)Convert.ChangeType(result, typeof(T));
        }

        // Execute text query and return a scalar value of type T
        public T ExecuteScalerStoredProcedure<T>(string queryString, Action<SqlParameterCollection> setParameters = null)
        {
            object result;
            using (SqlConnection connection = this.GetSqlConnectionInstance())
            {
                connection.Open();

                SqlCommand Command = new SqlCommand(queryString, connection);
                Command.CommandType = CommandType.StoredProcedure;
                Command.CommandTimeout = CommandTimeOut;
                setParameters(Command.Parameters);

                Command.Prepare();

                result = Command.ExecuteScalar();
            }

            return (T)Convert.ChangeType(result, typeof(T));
        }

        //Reader Methods
        public DataTable ExecuteReader(string commandName, Action<SqlParameterCollection> setParameters = null)
        {
            DataTable resultTable;
            using (SqlConnection connection = this.GetSqlConnectionInstance())
            {
                connection.Open();

                SqlCommand command = new SqlCommand(commandName, connection);
                command.CommandType = CommandType.StoredProcedure;
                command.CommandTimeout = CommandTimeOut;
                if (setParameters != null) setParameters(command.Parameters);

                command.Prepare();
                SqlDataReader reader = command.ExecuteReader();
                resultTable = new System.Data.DataTable();
                resultTable.Load(reader);
                reader.Close();
            }

            return resultTable;
        }

        public DataSet ExecuteProcedure(string commandName, Action<SqlParameterCollection> setParameters = null)
        {
            DataSet ds = new DataSet();

            using (SqlConnection conn = this.GetSqlConnectionInstance())
            {
                SqlCommand cmd = new SqlCommand(commandName, conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandTimeout = CommandTimeOut;
                if (setParameters != null) setParameters(cmd.Parameters);

                SqlDataAdapter da = new SqlDataAdapter();
                da.SelectCommand = cmd;
                conn.Open();
                da.Fill(ds);
                conn.Close();
            }
            return ds;
        }

        //Returns an output
        public T ExecuteNonQueryWithOutput<T>(string commandName, Action<SqlParameterCollection> setParameters = null)
        {
            using (SqlConnection connection = new SqlConnection(ConnectionString))
            {
                connection.Open();

                SqlCommand command = new SqlCommand(commandName, connection);
                command.CommandType = CommandType.StoredProcedure;
                command.CommandTimeout = CommandTimeOut;
                if (setParameters != null) setParameters(command.Parameters);

                command.Prepare();
                command.ExecuteNonQuery();

                return (T)command.Parameters[GetOutputParameter(command.Parameters)].Value;
            }
        }

        public void BulkCopy(string destinationTableName, DataTable source)
        {
            using (SqlConnection connection = new SqlConnection(ConnectionString))
            {
                connection.Open();
                using (SqlBulkCopy bulkCopy = new SqlBulkCopy(ConnectionString))
                {
                    bulkCopy.DestinationTableName = destinationTableName;
                    bulkCopy.BulkCopyTimeout = CommandTimeOut;
                    bulkCopy.ColumnMappings.Clear();
                    foreach (DataColumn col in source.Columns)
                    {
                        bulkCopy.ColumnMappings.Add(col.ColumnName.Trim(), col.ColumnName.Trim());
                    }
                    try
                    {
                        bulkCopy.WriteToServer(source);
                    }
                    catch (SqlException ex)
                    {
                        if (ex.Message.Contains("Received an invalid column length from the bcp client for colid"))
                        {
                            string pattern = @"\d+";
                            Match match = Regex.Match(ex.Message.ToString(), pattern);
                            var index = Convert.ToInt32(match.Value) - 1;

                            FieldInfo fi = typeof(SqlBulkCopy).GetField("_sortedColumnMappings", BindingFlags.NonPublic | BindingFlags.Instance);
                            var sortedColumns = fi.GetValue(bulkCopy);
                            var items = (Object[])sortedColumns.GetType().GetField("_items", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(sortedColumns);

                            FieldInfo itemdata = items[index].GetType().GetField("_metadata", BindingFlags.NonPublic | BindingFlags.Instance);
                            var metadata = itemdata.GetValue(items[index]);

                            var column = metadata.GetType().GetField("column", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance).GetValue(metadata);
                            var length = metadata.GetType().GetField("length", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance).GetValue(metadata);
                            //string Message = String.Format("Column: {0} contains data with a length greater than: {1}", column, length);
                            // throw Message;
                        }

                        throw;
                    }

                }
            }
        }


        public void BulkCopyPresistant(string destinationTableName, DataTable source)
        {
            // using (SqlConnection connection = new SqlConnection(ConnectionString))
            using (SqlBulkCopy bulkCopy = new SqlBulkCopy(_persistConnection))
            {
                bulkCopy.DestinationTableName = destinationTableName;
                bulkCopy.BulkCopyTimeout = CommandTimeOut;
                bulkCopy.ColumnMappings.Clear();
                foreach (DataColumn col in source.Columns)
                {
                    bulkCopy.ColumnMappings.Add(col.ColumnName.Trim(), col.ColumnName.Trim());
                }
                try
                {
                    bulkCopy.WriteToServer(source);
                }
                catch (SqlException ex)
                {
                    if (ex.Message.Contains("Received an invalid column length from the bcp client for colid"))
                    {
                        string pattern = @"\d+";
                        Match match = Regex.Match(ex.Message.ToString(), pattern);
                        var index = Convert.ToInt32(match.Value) - 1;

                        FieldInfo fi = typeof(SqlBulkCopy).GetField("_sortedColumnMappings", BindingFlags.NonPublic | BindingFlags.Instance);
                        var sortedColumns = fi.GetValue(bulkCopy);
                        var items = (Object[])sortedColumns.GetType().GetField("_items", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(sortedColumns);

                        FieldInfo itemdata = items[index].GetType().GetField("_metadata", BindingFlags.NonPublic | BindingFlags.Instance);
                        var metadata = itemdata.GetValue(items[index]);

                        var column = metadata.GetType().GetField("column", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance).GetValue(metadata);
                        var length = metadata.GetType().GetField("length", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance).GetValue(metadata);
                        //string Message = String.Format("Column: {0} contains data with a length greater than: {1}", column, length);
                        // throw Message;
                    }

                    throw;
                }

            }
        }

        public DataTable ExecuteODBCQuery(string connectionString, string query)
        {
            using (OdbcConnection connection = new OdbcConnection(connectionString))
            {
                connection.Open();
                var adapter = new OdbcDataAdapter(query, connection);
                var result = new DataTable();
                adapter.Fill(result);
                return result;
            }
        }

        public DataTable ExecuteExcelQuery(string filePath, bool hasHeaders = false, string sheetName = "")
        {
            if (!File.Exists(filePath)) throw new FileNotFoundException(filePath);

            DataTable dtexcel = new DataTable();
            string strConn = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + filePath + ";Extended Properties=\"Excel 12.0;HDR=" + (hasHeaders ? "YES" : "NO") + ";IMEX=0\"";

            OleDbConnection conn = new OleDbConnection(strConn);
            conn.Open();
            DataTable schemaTable = conn.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, new object[] { null, null, null, "TABLE" });

            string sheet = "";
            bool found = false;
            foreach (DataRow row in schemaTable.Rows)
            {
                sheet = row["TABLE_Name"].ToString();
                if (sheetName.Length == 0) { found = true; break; } // If no sheetname was specified then default to first worksheet.
                if (sheet.StartsWith(sheetName)) { found = true; break; } // Otherwise attempt to match the sheetname.
            }

            if (!found) throw new FileLoadException("Worksheet not found: " + sheetName);

            string query = "SELECT * FROM [" + sheet + "]";
            OleDbDataAdapter daexcel = new OleDbDataAdapter(query, conn);
            dtexcel.Locale = CultureInfo.CurrentCulture;
            daexcel.Fill(dtexcel);
            conn.Close();

            return dtexcel;
        }

        private string GetOutputParameter(SqlParameterCollection parameters)
        {
            foreach (SqlParameter param in parameters)
            {
                if (param.Direction == ParameterDirection.Output) return param.ParameterName;
            }
            throw new IndexOutOfRangeException("Expecting one output parameter. None found.");
        }

    }
}
