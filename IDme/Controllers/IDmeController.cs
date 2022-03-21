using IDme.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace IDme.Controllers
{
    public class IDmeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult Callback()
        {
            string verifiedreponse = "Sorry! Your ID.me account is not verified. Please try to verify your account again. Please click this link ";

            var callbackValue = Request.QueryString.ToString().ToLower().Contains("code");

            if (callbackValue)
            {
                var code = Request.QueryString["code"];

                var verified = enableProduction(code);

                if (verified)
                    verifiedreponse = "Thank you! Your ID.me account is verified. You will be redirected to our app to continue with your registration!";
            }
            else verifiedreponse = "Sorry! Your ID.me account is not verified. I didn't receive an authorization code from your ID.me account.";

            var responseModel = new responseModel()
            {
                response = verifiedreponse
            };

            return View(responseModel);
        }

        bool enableProduction(string code)
        {
            return authToken(code);
        }

        public bool authToken(string code)
        {
            string DestinationUrl = "https://api.id.me/oauth/token";

            var tokenBodyObj = new tokenBody()
            {
                code = code,
                code_verifier = "Write Random Code",
                client_id = "Your Client Id",
                client_secret = "Your Client Secret",
                grant_type = "authorization_code",
                redirect_uri = "Your Redirect URI"
            };

            string RequestJsonBody = JsonConvert.SerializeObject(tokenBodyObj);


            HttpWebResponse response;

            try
            {
                ServicePointManager.Expect100Continue = true;
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(DestinationUrl);
                byte[] bytes;
                bytes = Encoding.ASCII.GetBytes(RequestJsonBody);
                request.ContentType = "application/json";

                request.ContentLength = bytes.Length;
                request.Method = "POST";
                Stream requestStream = request.GetRequestStream();
                requestStream.Write(bytes, 0, bytes.Length);
                requestStream.Close();
                response = (HttpWebResponse)request.GetResponse();

                if (response.StatusCode == HttpStatusCode.OK)
                {
                    Stream responseStream = response.GetResponseStream();
                    string responseStr = new StreamReader(responseStream).ReadToEnd();

                    var responseTokenObj = JsonConvert.DeserializeObject<tokenResponse>(responseStr);
                    return attributes(responseTokenObj.access_token);
                }
                else
                {
                    return false;
                }
            }
            catch (Exception e)
            {
                return false;
            }
        }

        public bool attributes(string token)
        {
            string DestinationUrl = "https://api.id.me/api/public/v3/attributes.json?access_token=" + token;

            try
            {
                ServicePointManager.Expect100Continue = true;
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(DestinationUrl);
                request.Method = "GET";

                request.ContentType = "application/json";

                HttpWebResponse response = (HttpWebResponse)request.GetResponse();


                if (response.StatusCode == HttpStatusCode.OK)
                {
                    Stream responseStream = response.GetResponseStream();
                    string responseStr = new StreamReader(responseStream).ReadToEnd();

                    var responseTokenObj = JsonConvert.DeserializeObject<attributesResponse>(responseStr);

                    if (responseTokenObj.status[0].verified != null && responseTokenObj.status[0].verified.Value)
                    {
                        return true;
                    }
                    else return false;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception e)
            {
                return false;
            }
        }
    }
}