using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Framework.Common
{
    public class RegEx
    {
        //Invalid flag used for RegEx.
        bool invalid = false;
        /// <summary>
        /// RegEx function to check a string to see if it is a valid email address
        /// </summary>
        /// <param name="strIn"></param>
        /// <returns></returns>
        public bool IsValidEmail(string strIn)
        {
             invalid = false;
            if (String.IsNullOrEmpty(strIn))
                return false;

            // Use IdnMapping class to convert Unicode domain names. 
            try
            {
                strIn = Regex.Replace(strIn, @"(@)(.+)$", this.DomainMapper, RegexOptions.None);
            }
            catch (Exception)
            {
                return false;
            }

            if (invalid)
                return false;

            // Return true if strIn is in valid e-mail format. 
            try
            {
                return Regex.IsMatch(strIn,
                      @"^(?("")(""[^""]+?""@)|(([0-9a-z]((\.(?!\.))|[-!#\$%&'\*\+/=\?\^`\{\}\|~\w])*)(?<=[0-9a-z])@))" +
                      @"(?(\[)(\[(\d{1,3}\.){3}\d{1,3}\])|(([0-9a-z][-\w]*[0-9a-z]*\.)+[a-z0-9]{2,17}))$",
                      RegexOptions.IgnoreCase);
            }
            catch (Exception)
            {
                return false;
            }
        }

        public bool PasswordStrengh(string password)
        {
                /*
                ^                         Start anchor
                (?=.*[A-Z])               Ensure string has one uppercase letters.
                (?=.*[!@#$&*])            Ensure string has one special character.
                (?=.*[0-9])               Ensure string has one digit.
                (?=.*[a-z])               Ensure string has one lowercase letter.
                .{6-20}                   Ensure string is of length 6-20 characters.
                $                         End anchor.
             */
            try
            {
                return Regex.IsMatch(password, @"^(?=.*[A-Z])(?=.*[!@#$&*])(?=.*[0-9])(?=.*[a-z]).{6,20}$");
            }
            catch (Exception)
            {
                return false;
            }
        }

        private string DomainMapper(Match match)
        {
            // IdnMapping class with default property values.
            IdnMapping idn = new IdnMapping();

            string domainName = match.Groups[2].Value;
            try
            {
                domainName = idn.GetAscii(domainName);
            }
            catch (ArgumentException)
            {
                invalid = true;
            }
            return match.Groups[1].Value + domainName;
        }
    }
}
