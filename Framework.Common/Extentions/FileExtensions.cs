using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;
using System.Web;
using System.Text.Json;

namespace Framework.Common.Extensions
{
    public static class FileExtensions {

        /// <summary>
        /// Method that is used to Serialize a file into XML.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="toSerialize"></param>
        /// <returns></returns>
        public static string Serialize<T>(this T toSerialize)
        {
            XmlWriterSettings settings = new XmlWriterSettings();
            settings.ConformanceLevel = ConformanceLevel.Fragment;
            settings.Indent = false;
            settings.OmitXmlDeclaration = true;
            settings.NewLineChars = string.Empty;
            settings.NewLineHandling = NewLineHandling.None;

            var emptyNs = new XmlSerializerNamespaces(new[] { XmlQualifiedName.Empty });

            XmlSerializer xmlSerializer = new XmlSerializer(toSerialize.GetType());

            //StringWriter textWriter = new StringWriter();
            // XmlWriter writer = XmlWriter.Create(Console.Out, ws);

            using (StringWriter textWriter = new StringWriter())
            {
                using (var xw = XmlWriter.Create(textWriter, settings))
                {
                    // Build Xml with xw.
                    xmlSerializer.Serialize(textWriter, toSerialize, null);
                }

                return textWriter.ToString();
            }
        }

        /// <summary>
        /// Method to to turn an option into .JSON.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static string ToJSON<T>(this T obj)
        {
            return JsonSerializer.Serialize(obj);
        }


        public static T Deserialize<T>(this string toDeserialize) {
            XmlSerializer ser = new XmlSerializer(typeof(T));

            using (StringReader sr = new StringReader(toDeserialize)) {
                return (T)ser.Deserialize(sr);
            }
        }

        public static byte[] ToByteArray(this string val) {
            return Encoding.UTF8.GetBytes(val);
        }

        public static string FromByteArray(this byte[] val) {
            return Encoding.UTF8.GetString(val, 0, val.Length);
        }

        public static T Clone<T>(this T toClone) {
            var source = Serialize<T>(toClone);
            var cloned = Deserialize<T>(source);
            return cloned;
        }
    }
}
