using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace Framework.Common
{

    /// <summary>
    /// Log Modes Enum, This flagged enum can the multiple types of output for the logging class.
    /// </summary>
    [Flags]
    public enum LogModes : short
    {
        Console = 1,
        Text = 2,
        XML = 4,
        EventLog = 8,
        WMI = 16
    };

    /// <summary>
    /// Log verbosity Enum.
    /// </summary>
    public enum LogVerbosity
    {
        Debug,              //All Messages even Debug
        Normal,             //All Messages except Debug
        ErrorsWarnings,     //Only Errors and Warnings
        ErrorsOnly          //Only Errors
    };

    /// <summary>
    /// LogWriter class, Used to generate Logs.
    /// </summary>
    public class LogWriter
    {
        private object syncHandle = new object();                //Locking Object.

        private string eventLogName;                             //Event Log name, public so the user can specifiy log name if desired.
        public string EventLogName
        {
            get
            {
                lock (syncHandle)
                    return eventLogName;
            }
            set
            {
                if (string.IsNullOrEmpty(value))
                    throw new ArgumentException("Event Log Name cannot be blank", "EventLogName");
                lock (syncHandle)
                    eventLogName = value;
            }

        }


        private Queue<Log> m_logHistory;                        //Log history queue, currently unused, but I thought it might be handy to have.
        private LogModes m_Mode;                                //LogWriter Modes, the types of logging that the writer can output to.
        private string m_Source;                                //The name of the source of these logs.
        public LogVerbosity VerbosityLevel { get; set; }        //The verbosity level of the log writer, log entries that do not meet this threshold are ignored by the LogWriter.
        private static readonly object lock_ = new object();    //Log object, used for file writing.
        private string m_LogFilePath;                           //The log file path.

        /// <summary>
        /// LogWriter constructor
        /// </summary>
        /// <param name="source">The name of the application doing the logging.</param>
        /// <param name="mode">The types of logging output available, Such as Console, EventLog, XMLFile,etc.</param>
        /// <param name="VerbosityLevel">The level of logging we would like to report.</param>
        /// <param name="LogFilePath">If we are outputting to a log file the path of the log file can be specified here.</param>
        public LogWriter(string source = "Logging Libary", LogModes mode = LogModes.Console | LogModes.EventLog, LogVerbosity verbosityLevel = LogVerbosity.Normal, string LogFilePath = "", string LogName = "CustomLogging")
        {
            m_logHistory = new Queue<Log>();
            m_Mode = new LogModes();
            EventLogName = LogName;          //Folder in the Event log, could also be 'Application' if desired.

            //Set the parameters
            m_Mode = mode;
            m_Source = source;
            VerbosityLevel = verbosityLevel;

            //If we are built in Debug mode, we are going to output all the data.
#if DEBUG
            VerbosityLevel = LogVerbosity.Debug;
#endif


            m_LogFilePath = LogFilePath;
        }

        #region LogModes

        public LogModes getLogModes()
        {
            return m_Mode;
        }
        public void ClearLogModes()
        {
            m_Mode = new LogModes();
        }

        /// <summary>
        /// Sets the various logging modes. Flags for each of the modes, so that you can have multiple outputs.
        /// </summary>
        /// <param name="mode">Enum of the mode</param>
        /// <param name="value">Is mode enabled?</param>
        public void SetLogMode(LogModes mode, bool value)
        {
            if (value)
            {
                //If we are enabling it, flip the flag on the mode enum.
                m_Mode = m_Mode | mode;
            }
            else
            {
                //If we are disabling the flag, first check to make sure its enabled, then flip it off.
                if (m_Mode.HasFlag(mode))
                    m_Mode = m_Mode -= mode;
            }

        }

        #endregion
        #region Log Methods

        /// <summary>
        /// Main Logging function, Outputs the message to the configured modes
        /// </summary>
        /// <param name="sMessage">The Message to be recorded</param>
        public string Log(string sMessage, LogEntryType Type = LogEntryType.Information)
        {
            //Create new log class using the message
            Log NewEntry = new Log(sMessage, Type);

            switch (VerbosityLevel)
            {
                case LogVerbosity.Debug:
                    WriteLog(NewEntry); //If we are in debug mode, output everything
                    break;

                case LogVerbosity.Normal:
                    if (Type != LogEntryType.Debug) //Normally output everything but debug logging.
                        WriteLog(NewEntry);
                    break;

                case LogVerbosity.ErrorsWarnings: //Output only errors and warnings
                    if (Type != LogEntryType.Debug || Type != LogEntryType.Information || Type != LogEntryType.SuccessAudit)
                        WriteLog(NewEntry);
                    break;

                case LogVerbosity.ErrorsOnly:   //Output only errors
                    if (Type.Equals(LogEntryType.Error) || Type.Equals(LogEntryType.FailtureAduit))   //Errors only, outputs only errors.
                        WriteLog(NewEntry);
                    break;

            }
            //Return the logged message back.
            return sMessage;
        }
        /// <summary>
        /// Main Log write function
        /// </summary>
        /// <param name="log"></param>
        private void WriteLog(Log log)
        {
            //Add the log to the history queue.
            m_logHistory.Enqueue(log);

            //Check the modes of the writer
            LogModes modes = getLogModes();

            //For each of the modes, if its enabled, write to that log.
            if (modes.HasFlag(LogModes.Console))
                WriteConsole(log);

            if (modes.HasFlag(LogModes.EventLog))
                WriteToApplicationEventLog(log);

            if (modes.HasFlag(LogModes.Text))
                WriteToTextFile(log);

            if (modes.HasFlag(LogModes.WMI))
                WriteWMIEvent(log);

            if (modes.HasFlag(LogModes.XML))
                WriteXMLFile(log);
        }
        /// <summary>
        /// Write a debug level message to the log.
        /// </summary>
        /// <param name="sMessage">The message to be written</param>
        /// <returns></returns>
        public string Debug(string sMessage)
        {
            string Result = Log(sMessage, LogEntryType.Debug);
            return Result; //Return the string written to the log.
        }
        /// <summary>
        /// Write a infomational level message to the log.
        /// </summary>
        /// <param name="sMessage">>The message to be written</param>
        /// <returns></returns>
        public string Info(string sMessage)
        {
            string Result = Log(sMessage, LogEntryType.Information);
            return Result; //Return the string written to the log.
        }
        /// <summary>
        /// Write a warning level message to the log.
        /// </summary>
        /// <param name="sMessage">>The message to be written</param>
        /// <returns></returns>
        public string Warn(string sMessage)
        {
            string Result = Log(sMessage, LogEntryType.Warning);
            return Result;  //Return the string written to the log.
        }
        /// <summary>
        /// Write an error level message to the log.
        /// </summary>
        /// <param name="sMessage">>The message to be written</param>
        /// <returns></returns>
        public string Error(string sMessage)
        {
            string Result = Log(sMessage, LogEntryType.Error);
            return Result; //Return the string written to the log.
        }
        #endregion

        public void ChangeSource(string NewSource)
        {
            m_Source = NewSource;
        }


        #region Write Methods

        /// <summary>
        /// Function to format the log entry into output for the console.
        /// </summary>
        /// <param name="entry"></param>
        /// <returns></returns>
        private string WriteConsole(Log entry)
        {
            Console.WriteLine(entry.GetMessage());
            return (entry.GetMessage(false, false));
        }

        /// <summary>
        /// Function to output the log entry to the event log.
        /// </summary>
        /// <param name="entry">The Log class to be logged.</param>
        /// 

#pragma warning disable CA1416 // Validate platform compatibility, suppressing due to only running windows enviroments. 
        private void WriteToApplicationEventLog(Log entry)
        {
            try
            {
                //Check to see if the event log source exists, if it doesnt create it.

                if (!EventLog.SourceExists(m_Source))
                {
                    EventLog.CreateEventSource(m_Source, EventLogName);
                }

                // Create an EventLog instance and assign its source.
                EventLog.WriteEntry(m_Source, entry.GetMessage(false, false), entry.GetEventLogType());
            }
            catch (Exception exp)
            {
                if (!EventLog.SourceExists("LogWriter"))
                {
                    EventLog.CreateEventSource("LogWriter", "Application");
                }
                EventLog.WriteEntry("LogWriter", exp.Message);
            }
        }
#pragma warning restore CA1416 // Validate platform compatibility


        /// <summary>
        /// Writes the log entry to a text file with the date and time, default is in the running directory to a file named: log.txt
        /// </summary>
        /// <param name="entry">The Log entry to be outputted.</param>
        private void WriteToTextFile(Log entry)
        {
            System.IO.File.AppendAllText(m_LogFilePath, m_Source + ": " + entry.GetMessage() + "\n");
        }
        /// <summary>
        /// Write the log to a WMI channel.
        /// </summary>
        /// <param name="entry"></param>
        private void WriteWMIEvent(Log entry)
        {
            //TODO: Write the code that fires a WMI event when called.
        }
        /// <summary>
        /// Writes the log entry to a XML file.
        /// </summary>
        /// <param name="entry"></param>
        private void WriteXMLFile(Log entry)
        {
            ThreadWriteXML(entry);
        }
        /// <summary>
        /// Thread functions used to write XML file.
        /// </summary>
        /// <param name="entry">Log Entry file to be logged</param>
        private void ThreadWriteXML(Log entry)
        {
            string xmlPath = m_LogFilePath + m_Source + " Log.xml";
            XmlDocument XDoc = new XmlDocument();
            if (!File.Exists(xmlPath))
            {
                //If the file does not exist, create it.
                XDoc.LoadXml("<?xml version= \"1.0\"?> <?xml-stylesheet href=\"Log.xsl\" type=\"text/xsl\"?><Log App=\"" + m_Source + "\"></Log>");

            }
            else
                XDoc.Load(xmlPath);

            XmlNode root = XDoc.DocumentElement;

            //check to see if the entry date exists.
            string Query = "Date[@LogDate='" + entry.LogDate + "']";
            XmlNodeList dates = root.SelectNodes(Query);
            XmlElement date = null;
            if (dates.Count == 0)
            {
                date = XDoc.CreateElement("Date");
                date.SetAttribute("LogDate", entry.LogDate);
                root.AppendChild(date);
            }
            else
            {
                date = (XmlElement)dates[0];
            }

            //Parse the class and write to the xml file.
            XmlElement elemEntry = XDoc.CreateElement("Entry");
            elemEntry.SetAttribute("Time", entry.LogTime);
            elemEntry.SetAttribute("Source", m_Source);

            XmlElement elemMessage = XDoc.CreateElement("Message");
            elemMessage.InnerXml = entry.Message;
            XmlElement elemType = XDoc.CreateElement("Type");
            elemType.InnerXml = entry.EntryType.ToString();

            elemEntry.AppendChild(elemMessage);
            elemEntry.AppendChild(elemType);

            date.AppendChild(elemEntry);
            lock (lock_)
            {
                XDoc.Save(xmlPath);
            }

        }

    }
    #endregion
}
