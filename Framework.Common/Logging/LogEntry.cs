using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Framework.Common.Logging {
    public class LogEntry {
        public LogLevelType LogLevel { get; set; }
        public Exception Exception { get; set; }
        public string Message { get; set; }
        
        public LogEntry(LogLevelType logLevel, string message) : this(logLevel, message, null) { }
        public LogEntry(LogLevelType logLevel, Exception ex) : this(logLevel, string.Empty, ex) { }

        public LogEntry(LogLevelType logLevel, string message, Exception ex) {
            LogLevel = logLevel;
            Message = message;
            Exception = ex;
        }
    }
}
