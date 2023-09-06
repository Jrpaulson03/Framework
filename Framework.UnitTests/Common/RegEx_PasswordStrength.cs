using Framework.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Framework.UnitTests.Common
{
    [TestFixture]
    public class RegEx_PasswordStrength
    {

        private string ValidPassword;
        private string InvalidPassword;
        private RegEx RegExItem;

        [SetUp]
        public void SetUp()
        {
            RegExItem = new RegEx();
            ValidPassword = "Password123!";
            InvalidPassword = "pass";
        }

        [Test]
        public void RegEx_Email_IsValidPassword()
        {
            Assert.IsTrue(RegExItem.PasswordStrengh(ValidPassword));
        }

        [Test]
        public void RegEx_Email_IsInValidPassword()
        {
            Assert.IsFalse(RegExItem.PasswordStrengh(InvalidPassword));
        }
    }
}
