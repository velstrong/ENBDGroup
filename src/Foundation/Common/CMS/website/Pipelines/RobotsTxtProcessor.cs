using Glass.Mapper.Sc;
using Sitecore.Diagnostics;
using Sitecore.Pipelines.HttpRequest;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using Sitecore;
using Sitecore.Data.Items;
using ENBDGroup.Foundation.Common.Core.Utilities;
using ENBDGroup.Foundation.Common.Core.Extensions;
using Sitecore.Sites;
using Sitecore.Web;
using Sitecore.Configuration;
using System.Text.RegularExpressions;

namespace LivApp.Foundation.CMS.Pipelines
{
    public class RobotsTxtProcessor : HttpRequestProcessor
    {
        public override void Process(HttpRequestArgs args)
        {
            try
            {
                Assert.ArgumentNotNull(args, "args");

                if (!File.Exists(args.HttpContext.Request.PhysicalPath) && !Directory.Exists(args.HttpContext.Request.PhysicalPath))
                {

                    if (args.Url == null || !args.Url.FilePath.ToLower().EndsWith("robots.txt"))
                        return;

                    // Default robots.txt Content
                    string robotsTxtContent = @"User-agent: *" + Environment.NewLine + "Disallow: /sitecore";

                    var siteContext = GetSiteContext(HttpContext.Current.Request.Url);
                    Item settingsItem;

                    if (siteContext != null)
                    {
                        Sitecore.Data.Database master = Sitecore.Data.Database.GetDatabase("web");
                        settingsItem = master.GetItem(siteContext.RootPath);

                        if(settingsItem != null && !string.IsNullOrWhiteSpace(settingsItem.GetFieldValue("Robot Content")))
                        {
                            robotsTxtContent = settingsItem.GetFieldValue("Robot Content");
                        }
                    }

                    var response = HttpContext.Current.Response;
                    response.ContentType = "text/plain";
                    response.Write(robotsTxtContent);
                    //response.End();
                    //response.Flush();
                    //response.SuppressContent = true;
                    //HttpContext.Current.ApplicationInstance.CompleteRequest();
                }
            }
            catch (Exception ex)
            {
                Sitecore.Diagnostics.Log.Error("Error - robots.txt", ex, this);
            }
        }
        public static SiteContext GetSiteContext(Uri requestUrl)
        {
            Assert.ArgumentNotNull(requestUrl, "requestUrl");
            string requestHostName = requestUrl.Host;

            foreach (SiteInfo siteInfo in Factory.GetSiteInfoList())
            {
                if (IsMatch(requestHostName, siteInfo.HostName) || IsMatch(requestHostName, siteInfo.TargetHostName))
                {
                    return new SiteContext(siteInfo);
                }
            }

            return SiteContext.GetSite("website");
        }

        private static bool IsMatch(string input, string wildcardPattern)
        {
            if (string.IsNullOrWhiteSpace(input) || string.IsNullOrWhiteSpace(wildcardPattern))
            {
                return false;
            }

            string regexPattern = WildcardToRegex(wildcardPattern);
            return Regex.IsMatch(input, regexPattern, RegexOptions.IgnoreCase);
        }
        private static string WildcardToRegex(string pattern)
        {
            return "^" + Regex.Escape(pattern).Replace("*", ".*").Replace("?", ".") + "$";
        }
    }
}