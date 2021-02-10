using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;

namespace ENBDGroup.Foundation.Common.Core.Utilities
{
    public static class HttpUtility
    {
        public static string InstagramData(string url, string tokenUrl)
        {
            string data = string.Empty;
            try
            {
                try
                {
                    string access_token = string.Empty;
                    if (!string.IsNullOrEmpty(tokenUrl))
                    {
                        HttpWebRequest httpWebRequest1 = (HttpWebRequest)WebRequest.Create(string.Format(tokenUrl));
                        httpWebRequest1.Method = "GET";
                        httpWebRequest1.ContentType = "application/json";
                        httpWebRequest1.UserAgent = "Mozilla/4.0 (compatible; MSIE 7.0; Windows NT 5.1; .NET CLR 2.0.50727; .NET4.0C; .NET4.0E)";

                        using (HttpWebResponse httpWebResponse = (HttpWebResponse)httpWebRequest1.GetResponse())
                        {
                            using (Stream stream = httpWebResponse.GetResponseStream())
                            {
                                using (StreamReader streamReader = new StreamReader(stream))
                                {
                                    access_token = (Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(streamReader.ReadToEnd()).access_token).Value;
                                }
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Sitecore.Diagnostics.Log.Error("Error while Calling API:" + url, ex, Type.EmptyTypes);
                }
                
                HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(url);
                httpWebRequest.Method = "GET";
                httpWebRequest.ContentType = "application/json";
                using (HttpWebResponse httpWebResponse = (HttpWebResponse)httpWebRequest.GetResponse())
                {
                    using (Stream stream = httpWebResponse.GetResponseStream())
                    {
                        using (StreamReader streamReader = new StreamReader(stream))
                        {
                            data = streamReader.ReadToEnd();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Sitecore.Diagnostics.Log.Error("Error while Calling API:" + url, ex, Type.EmptyTypes);
            }
            return data;
        }
        public static string FacebookData(string url, string clientid, string clientsecret, int limit = 8)
        {
            string data = string.Empty;

            try
            {
                string access_token = string.Empty;
                try
                {
                    string accessTokenUrl = "https://graph.facebook.com/oauth/access_token?client_id={0}&client_secret={1}&grant_type=client_credentials";

                    HttpWebRequest httpWebRequest1 = (HttpWebRequest)WebRequest.Create(string.Format(accessTokenUrl, clientid, clientsecret));
                    httpWebRequest1.Method = "GET";
                    httpWebRequest1.ContentType = "application/json";
                    httpWebRequest1.UserAgent = "Mozilla/4.0 (compatible; MSIE 7.0; Windows NT 5.1; .NET CLR 2.0.50727; .NET4.0C; .NET4.0E)";

                    using (HttpWebResponse httpWebResponse = (HttpWebResponse)httpWebRequest1.GetResponse())
                    {
                        using (Stream stream = httpWebResponse.GetResponseStream())
                        {
                            using (StreamReader streamReader = new StreamReader(stream))
                            {
                                //access_token = "229138694600829|e1203bcb5f1fdba4798f502b1aeba344";
                                access_token = (Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(streamReader.ReadToEnd()).access_token).Value;
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Sitecore.Diagnostics.Log.Error("Error while Calling API:" + url, ex, Type.EmptyTypes);
                }

                HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(url + "?access_token=" + access_token + "&fields=id,full_picture,created_time,from{id,name,picture},message,link,type,shares,object_id,attachments&limit=" + limit);
                httpWebRequest.Method = "GET";
                httpWebRequest.ContentType = "application/json";
                httpWebRequest.UserAgent = "Mozilla/4.0 (compatible; MSIE 7.0; Windows NT 5.1; .NET CLR 2.0.50727; .NET4.0C; .NET4.0E)";

                using (HttpWebResponse httpWebResponse = (HttpWebResponse)httpWebRequest.GetResponse())
                {
                    using (Stream stream = httpWebResponse.GetResponseStream())
                    {
                        using (StreamReader streamReader = new StreamReader(stream))
                        {
                            data = streamReader.ReadToEnd();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Sitecore.Diagnostics.Log.Error("Error while Calling API:" + url, ex, Type.EmptyTypes);
            }
            return data;
        }

        public static string YoutubeData(string url, string accessToken, string playlistId, string pageToken, int limit = 8)
        {
            string data = string.Empty;
            try
            {
                string requestUrl = url + string.Format("?part=snippet&playlistId={0}&maxResults={1}&key={2}", playlistId, limit, accessToken);
                if (!string.IsNullOrEmpty(pageToken))
                {
                    requestUrl = requestUrl + "&pageToken=" + pageToken;
                }
                HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(requestUrl);
                httpWebRequest.Method = "GET";
                httpWebRequest.ContentType = "application/json";
                httpWebRequest.UserAgent = "Mozilla/4.0 (compatible; MSIE 7.0; Windows NT 5.1; .NET CLR 2.0.50727; .NET4.0C; .NET4.0E)";

                using (HttpWebResponse httpWebResponse = (HttpWebResponse)httpWebRequest.GetResponse())
                {
                    using (Stream stream = httpWebResponse.GetResponseStream())
                    {
                        using (StreamReader streamReader = new StreamReader(stream))
                        {
                            data = streamReader.ReadToEnd();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Sitecore.Diagnostics.Log.Error("Error while Calling API:" + url, ex, Type.EmptyTypes);
            }
            return data;
        }

        public static dynamic GetJsonResponse(string url)
        {
            dynamic jsonResponse = null;
            try
            {
                HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(url);
                httpWebRequest.Method = "GET";
                httpWebRequest.ContentType = "application/json";
                using (HttpWebResponse httpWebResponse = (HttpWebResponse)httpWebRequest.GetResponse())
                {
                    using (Stream stream = httpWebResponse.GetResponseStream())
                    {
                        using (StreamReader streamReader = new StreamReader(stream))
                        {
                            jsonResponse = JsonConvert.DeserializeObject(streamReader.ReadToEnd());
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Sitecore.Diagnostics.Log.Error("Error while Calling API:" + url, ex, Type.EmptyTypes);
            }
            return jsonResponse;
        }

        public static string GetCookie(string key)
        {
            string value = string.Empty;
            if (!string.IsNullOrEmpty(key))
            {
                if (HttpContext.Current.Request.Cookies[key] != null)
                {
                    HttpCookie aCookie = HttpContext.Current.Request.Cookies[key];
                    //value = HttpContext.Current.Server.HtmlEncode(aCookie.Value);
                    return aCookie.Value;
                }
            }
            return value;
        }

        public static void CreateUpdateCookie(string key, string value, int days = 1)
        {
            HttpCookie aCookie = new HttpCookie(key);
            if (!string.IsNullOrEmpty(value) && !string.IsNullOrEmpty(key))
            {
                if (HttpContext.Current.Request.Cookies[key] != null)
                {
                    HttpContext.Current.Request.Cookies.Remove(key);
                }
                aCookie.Value = value;
                aCookie.Expires = DateTime.UtcNow.AddDays(days);
                aCookie.HttpOnly = true;
                aCookie.Secure = true;
                aCookie.SameSite = SameSiteMode.Strict;
                HttpContext.Current.Response.Cookies.Add(aCookie);
            }
        }
        public static bool IsMobileBrowser()
        {
            //GETS THE CURRENT USER CONTEXT
            HttpContext context = HttpContext.Current;

            //FIRST TRY BUILT IN ASP.NT CHECK
            if (context.Request.Browser.IsMobileDevice)
            {
                return true;
            }
            //THEN TRY CHECKING FOR THE HTTP_X_WAP_PROFILE HEADER
            if (context.Request.ServerVariables["HTTP_X_WAP_PROFILE"] != null)
            {
                return true;
            }
            //THEN TRY CHECKING THAT HTTP_ACCEPT EXISTS AND CONTAINS WAP
            if (context.Request.ServerVariables["HTTP_ACCEPT"] != null &&
                context.Request.ServerVariables["HTTP_ACCEPT"].ToLower().Contains("wap"))
            {
                return true;
            }
            //AND FINALLY CHECK THE HTTP_USER_AGENT 
            //HEADER VARIABLE FOR ANY ONE OF THE FOLLOWING
            if (context.Request.ServerVariables["HTTP_USER_AGENT"] != null)
            {
                //Create a list of all mobile types
                string[] mobiles =
                    new[]
                        {
                    "midp", "j2me", "avant", "docomo",
                    "novarra", "palmos", "palmsource",
                    "240x320", "opwv", "chtml",
                    "pda", "windows ce", "mmp/",
                    "blackberry", "mib/", "symbian",
                    "wireless", "nokia", "hand", "mobi",
                    "phone", "cdm", "up.b", "audio",
                    "SIE-", "SEC-", "samsung", "HTC",
                    "mot-", "mitsu", "sagem", "sony"
                    , "alcatel", "lg", "eric", "vx",
                    "NEC", "philips", "mmm", "xx",
                    "panasonic", "sharp", "wap", "sch",
                    "rover", "pocket", "benq", "java",
                    "pt", "pg", "vox", "amoi",
                    "bird", "compal", "kg", "voda",
                    "sany", "kdd", "dbt", "sendo",
                    "sgh", "gradi", "jb", "dddi",
                    "moto", "iphone"
                        };

                //Loop through each item in the list created above 
                //and check if the header contains that text
                foreach (string s in mobiles)
                {
                    if (context.Request.ServerVariables["HTTP_USER_AGENT"].
                                                        ToLower().Contains(s.ToLower()))
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        public static bool IsAndroid()
        {
            var agent = HttpContext.Current.Request.UserAgent.ToLower();
            return agent.Contains("android");
        }
    }
}