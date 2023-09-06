using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Framework.Common
{
    /// <summary>
    /// LogEntryType enum
    /// </summary>
    public enum LogEntryType
    {
        Error,
        Warning,
        Information,
        SuccessAudit,
        FailtureAduit,
        Debug
    }

    /// <summary>
    /// A Log class to store the message and the Date and Time the log entry was created
    /// </summary>
    public class Log
    {
        //Getters and setters.
        public string Message { get; set; }
        public string LogTime { get; set; }
        public string LogDate { get; set; }
        public LogEntryType EntryType { get; set; }
        public override string ToString()
        {
            return Message;
        }
        /// <summary>
        /// Log class ctor.
        /// </summary>
        /// <param name="message">The log message</param>
        /// <param name="type">The type of the log</param>
        public Log(string message, LogEntryType type = LogEntryType.Information)
        {
            Message = message;
            LogDate = DateTime.Now.ToString("yyyy-MM-dd");
            LogTime = DateTime.Now.ToString("hh:mm:ss.fff tt");
            EntryType = type;
        }


        /// <summary>
        /// Used for writing the log entry to the eventlog, converts the LogEntry type from the enum to the System.Diagnostics EventLogType. This does tie the libary to be used on Windows, suppressing the warning due to only running windows enviroments.
        /// </summary>
        /// <returns></returns>

        #pragma warning disable CA1416 // Validate platform compatibility
        public EventLogEntryType GetEventLogType()
        {

            EventLogEntryType type = new EventLogEntryType();


            switch (EntryType)
            {
                case LogEntryType.Error: type = EventLogEntryType.Error; break;

                case LogEntryType.FailtureAduit: type = EventLogEntryType.FailureAudit; break;
                case LogEntryType.Information: type = EventLogEntryType.Information; break;
                case LogEntryType.Debug: type = EventLogEntryType.Information; break;
                case LogEntryType.SuccessAudit: type = EventLogEntryType.SuccessAudit; break;
                case LogEntryType.Warning: type = EventLogEntryType.Warning; break;

                default: break;

            }

            return type;
        }
        #pragma warning restore CA1416 // Validate platform compatibility
        
        /// <summary>
        /// Format the message with date and time.
        /// </summary>
        /// <param name="date"></param>
        /// <param name="time"></param>
        /// <returns></returns>
        public string GetMessage(bool date = true, bool time = true)
        {
            StringBuilder sb = new StringBuilder();
            if (date)
            {
                sb.Append(LogDate);
                sb.Append(" - ");
            }
            if (time)
            {
                sb.Append(LogTime);
                sb.Append(" - ");
            }
            sb.Append(Message);

            return sb.ToString();
        }
    }
}
