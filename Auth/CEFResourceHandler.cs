using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using CefSharp;
using CefSharp.Handler;

namespace Project_127.Auth
{
    class CEFResourceHandler : ResourceRequestHandler
    {
        /*protected override bool OnResourceResponse(IWebBrowser browserControl, IBrowser browser, IFrame frame, IRequest request, IResponse response)
        {
            HelperClasses.Logger.Log(request.Url);
            return false;
        }*/

        private static HttpClient httpClient = new HttpClient();

        protected override IResourceHandler GetResourceHandler(IWebBrowser chromiumWebBrowser, IBrowser browser, IFrame frame, IRequest request)
        {
            
            // Download the JS
            var req = new HttpRequestMessage
            {
                RequestUri = new Uri(request.Url),
                Method = HttpMethod.Get,
            };

			string res = "";
			try
			{
				res = httpClient.SendAsync(req).Result.Content.ReadAsStringAsync().Result;
			}
			catch (Exception e)
			{
				HelperClasses.Logger.Log("First http post inside CefResouceHandler shit the bed");
				HelperClasses.Logger.Log("e.ToString():\n" + e.ToString(), true, 1);
				HelperClasses.Logger.Log("e.Message.ToString():\n" + e.Message.ToString(), true, 1);
				HelperClasses.Logger.Log("e.InnerException.ToString():\n" + e.InnerException.ToString(), true, 1);
			}

            var modRes = Regex.Replace(res, @"(t.isDlcTitleInfoSupported=function\(e\)\{)", "$1return false;");
            // since we cant mod the response...
            //System.Windows.MessageBox.Show(modRes.Length.ToString());
            Task.Delay(5000);
            frame.ExecuteJavaScriptAsync(modRes, request.Url, 0);
            //frame.ExecuteJavaScriptAsync("alert(\"fired\");");
            // this intentionally errors. (CORS issue)
            return ResourceHandler.FromString("/*edited*/", mimeType: Cef.GetMimeType("js"));
        }
    }
    public class CEFRequestHandler : RequestHandler
    {
        protected override IResourceRequestHandler GetResourceRequestHandler(IWebBrowser chromiumWebBrowser, IBrowser browser, IFrame frame, IRequest request, bool isNavigation, bool isDownload, string requestInitiator, ref bool disableDefaultHandling)
        {
            //HelperClasses.Logger.Log("In GetResourceRequestHandler : " + request.Url);

            //gamedownloads\.rockstargames\.com\/mtl\/ext\/mtl\/.+\/js\/build\/launcher\.[\da-f]+\.js
            //Only intercept specific Url
            var pattern = @"gamedownloads\.rockstargames\.com\/mtl\/ext\/mtl\/.+\/js\/build\/launcher\.[\da-f]+\.js";
            if (Regex.Match(request.Url, pattern).Success)
            {
                return new CEFResourceHandler();
            }
            ///Default behaviour, url will be loaded normally.
            return null;
        }

    }
}
