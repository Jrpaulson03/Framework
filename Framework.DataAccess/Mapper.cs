using System;
using System.Data;
using System.Linq;
using System.Runtime.Remoting;

namespace Framework.DataAccess {
    public static class Mapper {

        /// <summary>
        /// Discovers a Mapping file based on convention of "TypeName" + "Mapper"
        /// ie. "Account" type would discover its mapping file "AccountMapper"
        /// </summary>
        /// <typeparam name="T">Return type.</typeparam>
        /// <typeparam name="U">Type of the mapping file to discover.</typeparam>
        /// <typeparam name="W">Type of the data being passed in to mapper.</typeparam>
        /// <param name="data">Data is the type defined by generic parameter W</param>
        /// <returns>Object of Type T</returns>
        public static T Map<T, U, W>(W data) {
            var fullName = typeof(U).FullName + "Mapper";
            var mapperName = fullName.Replace("Models", "Mappers");

            ObjectHandle handle = Activator.CreateInstance(typeof(U).Assembly.FullName, mapperName);
            IMapper mapper = (IMapper) handle.Unwrap();

            T mappedObject = mapper.Map<T, W>(data);
            return mappedObject;
        }

        /// <summary>
        /// Helper function for DataRows extraction.
        /// Attempts extract field value from data row.
        /// Checks to see if field exists and if value is not dbNull.
        /// </summary>
        /// <typeparam name="T">Type of value expected back.</typeparam>
        /// <param name="row">Data row to be passed in.</param>
        /// <param name="field">Name of column to extract value from.</param>
        /// <returns>Object of Type T</returns>
        public static T Parse<T>(DataRow row, string field, T defaultValue = default(T)) {
            var result = defaultValue;
            if (!row.Table.Columns.Contains(field)) return result;

            var item = row[field];
            if (item == DBNull.Value) return result;

            var t = typeof(T);
            if (t.IsGenericType && t.GetGenericTypeDefinition().Equals(typeof(Nullable<>))) {
                if (item == null) {
                    return result;
                }

                t = Nullable.GetUnderlyingType(t);
            }

            try {
                result = (T)Convert.ChangeType(item, t);
            }
            catch {
                // Log error - InvalidCastException.
            }
            return result;
        }
    }
}
