using System.Collections.Specialized;
using System.Net;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;


namespace Framework.Web
{
    public static class WebHelper
    {


        public static T HttpGet<T>(string requesturi)
        {
            string jsonResponse = null;
            using (WebClient client = new WebClient())
            {
                var response = client.DownloadData(requesturi);
                jsonResponse = Encoding.UTF8.GetString(response);
            }

            T obj = JsonSerializer.Deserialize<T>(jsonResponse);

            return obj;
        }

        public static string HttpGet(string requesturi)
        {
            string jsonResponse = "";
            using (WebClient client = new WebClient())
            {
                var response = client.DownloadData(requesturi);
                jsonResponse = Encoding.UTF8.GetString(response);
            }
            return jsonResponse;
        }

        public static string ToJSON(this object item)
        {
            return JsonSerializer.Serialize(item);
        }

        public static string HttpPostData(string requesturi, string jsonData)
        {
            string responseJson = "";
            using (WebClient client = new WebClient())
            {
                client.Headers[HttpRequestHeader.Accept] = "application/json";
                client.Headers[HttpRequestHeader.ContentType] = "application/json";

                byte[] response = client.UploadData(requesturi, Encoding.UTF8.GetBytes(jsonData));
                responseJson = Encoding.UTF8.GetString(response);
            }
            return responseJson;
        }

        public static T HttpPost<T>(string requesturi, string jsonData)
        {

            string jsonResponse = null;
            //ServicePointManager.ServerCertificateValidationCallback = delegate { return true; };
            using (WebClient client = new WebClient())
            {
                client.Headers[HttpRequestHeader.Accept] = "application/json";
                client.Headers[HttpRequestHeader.ContentType] = "application/json";

                var responseBytes = client.UploadData(requesturi, Encoding.UTF8.GetBytes(jsonData));
                jsonResponse = Encoding.UTF8.GetString(responseBytes);
            }

            T obj = JsonSerializer.Deserialize<T>(jsonResponse);


            return obj;
        }

        public static void HttpPost(string requesturi, string jsonData)
        {
            using (WebClient client = new WebClient())
            {
                client.Headers[HttpRequestHeader.Accept] = "application/json";
                client.Headers[HttpRequestHeader.ContentType] = "application/json";

                var responseBytes = client.UploadData(requesturi, Encoding.UTF8.GetBytes(jsonData));
            }
        }


        public static void HttpPost(string requesturi, NameValueCollection parameters)
        {
            //string jsonResponse = null;
            //ServicePointManager.ServerCertificateValidationCallback = delegate { return true; };
            using (WebClient client = new WebClient())
            {
                var responseBytes = client.UploadValues(requesturi, "POST", parameters);
            }
        }

        public static Stream GetFileStream(string fileUrl)
        {
            HttpWebRequest webRequest = (HttpWebRequest)WebRequest.Create(fileUrl);
            webRequest.UseDefaultCredentials = true;
            webRequest.PreAuthenticate = true;
            webRequest.Credentials = CredentialCache.DefaultCredentials;

            HttpWebResponse webResponse = (HttpWebResponse)webRequest.GetResponse();
            return webResponse.GetResponseStream();
        }

        //public static String GetHtmlContent(string url, HttpCookie cookie)
        //{
        //    var cookieJar = new CookieContainer();
        //    cookieJar.Add(cookie.ToNetCookie());

        //    var client = new CookieAwareWebClient(cookieJar);
        //    var content = client.DownloadString(url);
        //    return content;
        //}

        //public static Cookie ToNetCookie(this HttpCookie c)
        //{
        //    var n = new Cookie();
        //    n.Domain = c.Domain;
        //    n.Expires = c.Expires;
        //    n.HttpOnly = c.HttpOnly;
        //    n.Name = c.Name;
        //    n.Path = c.Path;
        //    n.Secure = c.Secure;
        //    n.Value = c.Value;
        //    return n;
        //}

    }
}