using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Framework.Web
{
    public enum WebHelperResponseStatus
    {
        Success,
        Fail,
        CompletedWithErrors
    }

    public class WebHelperResponse
    {

        public WebHelperResponseStatus ResponseStatus { get; set; }
        public string ResponseStatusMessage { get; set; }
        public string Response { get; set; }


        public WebHelperResponse()
        {
            ResponseStatus = WebHelperResponseStatus.Success;
            ResponseStatusMessage = "";

        }
        


    }
}
