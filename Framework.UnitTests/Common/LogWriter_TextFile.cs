using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Framework.Common;
using NUnit.Framework;

namespace Framework.UnitTests.Common
{

    [TestFixture]
    public class LogWriter_TextFile
    {
        private LogWriter _logger;
        private string logfilePath;

        [SetUp]
        public void SetUp()
        {
            logfilePath = Path.GetFileName("UnitTestLog.txt");
            
            _logger = new LogWriter("Framework UnitTest", LogModes.Text,LogVerbosity.Normal, logfilePath);
     

            if (File.Exists(logfilePath))
            {
                File.Delete(logfilePath);
            }

        }

        [Test]
        public void Logwriter_TextFile_Info()
        {
        
                _logger.Info("This is a test message for logging, set to the info level.");
          
                Assert.That(logfilePath, Does.Exist);
        }
    }
 }
