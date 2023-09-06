using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Framework.Common.Logging {
    public static class LoggingExtensions {

        public static void Log(this ILogger logger, LogLevelType logLevel, string message) {
            var logEntry = new LogEntry(logLevel, message);
            logger.Log(logEntry);
        }

        public static void Log(this ILogger logger, LogLevelType logLevel, Exception ex) {
            var logEntry = new LogEntry(logLevel, ex);
            logger.Log(logEntry);
        }

        public static void Log(this ILogger logger, LogLevelType logLevel, string message, Exception ex) {
            var logEntry = new LogEntry(logLevel, message, ex);
            logger.Log(logEntry);
        }

        public static void Trace(this ILogger logger, string message) {
            logger.Log(LogLevelType.Trace, message);
        }

        public static void Trace(this ILogger logger, Exception ex) {
            logger.Log(LogLevelType.Trace, ex);
        }

        public static void Debug(this ILogger logger, string message) {
            logger.Log(LogLevelType.Debug, message);
        }

        public static void Debug(this ILogger logger, Exception ex) {
            logger.Log(LogLevelType.Debug, ex);
        }

        public static void Info(this ILogger logger, string message) {
            logger.Log(LogLevelType.Info, message);
        }

        public static void Info(this ILogger logger, Exception ex) {
            logger.Log(LogLevelType.Info, ex);
        }

        public static void Warn(this ILogger logger, string message) {
            logger.Log(LogLevelType.Warn, message);
        }

        public static void Warn(this ILogger logger, Exception ex) {
            logger.Log(LogLevelType.Warn, ex);
        }

        public static void Error(this ILogger logger, string message) {
            logger.Log(LogLevelType.Error, message);
        }

        public static void Error(this ILogger logger, Exception ex) {
            logger.Log(LogLevelType.Error, ex);
        }

        public static void Fatal(this ILogger logger, string message) {
            logger.Log(LogLevelType.Fatal, message);
        }

        public static void Fatal(this ILogger logger, Exception ex) {
            logger.Log(LogLevelType.Fatal, ex);
        }
    }
}
