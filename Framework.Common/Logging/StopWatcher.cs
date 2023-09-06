using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Framework.Common {
    public class StopWatcher {
        private Stopwatch _stopWatch;
        private TimeSpan _elapsed;

        public StopWatcher Start() {
            if (_stopWatch == null) _stopWatch = new Stopwatch();
            _elapsed = new TimeSpan();
            _stopWatch.Reset();
            _stopWatch.Start();
            return this;
        }

        public StopWatcher Stop() {
            _elapsed = _stopWatch.Elapsed;
            return this;
        }

        public int ToSeconds() {
            return (int)_elapsed.TotalSeconds;
        }

        public int ToMilliseconds() {
            return (int)_elapsed.TotalMilliseconds;
        }

        public int ToMinutes() {
            return (int)_elapsed.TotalMinutes;
        }

        public override string ToString() {
            return String.Format("{0:0}:{1:00}.{2:00} secs", _elapsed.Minutes, _elapsed.Seconds, _elapsed.Milliseconds/10);            
        }
    }
}
