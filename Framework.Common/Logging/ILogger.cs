using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Framework.Common.Logging {
    public interface ILogger {
        string Source { get; set; }
        void Log(LogEntry entry);
    }
}
