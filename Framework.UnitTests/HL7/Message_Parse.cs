using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Framework.UnitTests.HL7
{
    [TestFixture]
    public class Message_Parse
    {

        private string HL7_Message;
   

        [SetUp]
        public void SetUp()
        {
            string path = "HL7Test.txt";
            HL7_Message = File.ReadAllText(path);

        }

        /// <summary>
        /// Test to parse a HL7 File, from here you can do various things with the parsed file structure.
        /// </summary>
        [Test]
        public void Message_ParseTest()
        {
            Framework.HL7.Models.Message message = new Framework.HL7.Models.Message(HL7_Message);

            Assert.DoesNotThrow(() => {
                message.ParseMessage();
            });

        }


    }
}
