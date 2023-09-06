using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Framework.Common.Logging {
    public class NLogImplementation : ILogger {
        private Logger NLogger { get; set; }
        public string Source { get; set; }

        public NLogImplementation() { }
        public NLogImplementation(string source) { Source = source; }

        public static ILogger Create() {
            return new NLogImplementation();
        }

        public static ILogger Create(string source) {
            return new NLogImplementation(source);
        }

        public void Log(LogEntry entry) {
            if (NLogger == null) {                                
                NLogger = LogManager.GetLogger(Source);
            }

            if (entry.LogLevel == LogLevelType.Trace) { Trace(entry); }
            if (entry.LogLevel == LogLevelType.Debug) { Debug(entry); }
            if (entry.LogLevel == LogLevelType.Info) { Info(entry); }
            if (entry.LogLevel == LogLevelType.Warn) { Warn(entry); }
            if (entry.LogLevel == LogLevelType.Error) { Error(entry); }
            if (entry.LogLevel == LogLevelType.Fatal) { Fatal(entry); }
        }

        private void Trace(LogEntry entry) {
            NLogger.Trace(entry.Message, entry.Exception);            
        }

        private void Debug(LogEntry entry) {
            NLogger.Debug(entry.Message, entry.Exception);
        }

        private void Info(LogEntry entry) {
            NLogger.Info(entry.Message, entry.Exception);
        }

        private void Warn(LogEntry entry) {
            NLogger.Warn(entry.Message, entry.Exception);
        }

        private void Error(LogEntry entry) {
            NLogger.Error(entry.Message, entry.Exception);
        }

        private void Fatal(LogEntry entry) {
            NLogger.Fatal(entry.Message, entry.Exception);
        }
    }
}
