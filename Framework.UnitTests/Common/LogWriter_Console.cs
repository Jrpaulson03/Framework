using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Framework.Common;
using Framework.Common.Logging;
using NUnit.Framework;


namespace Framework.UnitTests.Common
{
    [TestFixture]
    public class LogWriter_Console
    {
        private LogWriter _logger;

        [SetUp]
        public void SetUp()
        {
            _logger = new LogWriter("Framework UnitTest",LogModes.Console);
        }

        [Test]
        public void Logwriter_WriteInfo_NoExceptions()
        {
            Assert.DoesNotThrow(() => {
                _logger.Info("This is a test message for logging, set to the info level.");

            });
        }

        [Test]
        public void Logwriter_WriteWarning_NoExceptions()
        {
            Assert.DoesNotThrow(() => {
                _logger.Warn("This is a test message for logging, set to the warn level.");

            });
        }

        [Test]
        public void Logwriter_WriteDebug_NoExceptions()
        {
            Assert.DoesNotThrow(() => {
                _logger.Debug("This is a test message for logging, set to the debug level.");

            });
        }

        [Test]
        public void Logwriter_WriteError_NoExceptions()
        {
            Assert.DoesNotThrow(() => {
                _logger.Error("This is a test message for logging, set to the error level.");

            });
        }
    
    }
}
