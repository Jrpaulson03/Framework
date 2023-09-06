using Framework.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Framework.UnitTests.Common
{
    [TestFixture]
    public class RegEx_Email
    {

        private string ValidEmail;
        private string InvalidEmail;
        private RegEx RegExItem;

        [SetUp]
        public void SetUp()
        {
            RegExItem = new RegEx();
            ValidEmail = "Test@Test.com";
            InvalidEmail = "asdf.com";
        }

        [Test]
        public void RegEx_Email_IsValidEmail()
        {
            Assert.IsTrue(RegExItem.IsValidEmail(ValidEmail));
        }

        [Test]
        public void RegEx_Email_IsInValidEmail()
        {
            Assert.IsFalse(RegExItem.IsValidEmail(InvalidEmail));
        }
    }
}
