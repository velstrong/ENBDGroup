using Microsoft.Extensions.DependencyInjection;
using Sitecore.Abstractions;
using Sitecore.DependencyInjection;
using Sitecore.Diagnostics;
using Sitecore.Pipelines.HttpRequest;
using Sitecore.Sites;
using Sitecore.Web;
using System;

namespace ENBDGroup.Foundation.Common.CMS.Pipelines
{
    public class ENBDSiteResolver : SiteResolver
    {
        public ENBDSiteResolver() : this(ServiceLocator.ServiceProvider.GetRequiredService<BaseSiteContextFactory>())
        {
        }
        public ENBDSiteResolver(BaseSiteContextFactory siteContextFactory) : base(siteContextFactory)
        {
        }
        protected ENBDSiteResolver(BaseSiteContextFactory siteContextFactory, bool enableSiteConfigFiles) : base(siteContextFactory, enableSiteConfigFiles)
        {
        }
        public override void Process(HttpRequestArgs args)
        {
            Assert.ArgumentNotNull(args, "args");
            SiteContext site = CustomResolveSiteContext(args);
            this.UpdatePaths(args, site);
            this.SetSiteToRequestContext(site);
        }

        protected virtual SiteContext CustomResolveSiteContext(HttpRequestArgs args)
        {
            SiteContext siteContext = null;
            string queryString = this.GetQueryString(this.SiteQueryStringKey, args);
            if (queryString.Length > 0)
            {
                siteContext = this.SiteContextFactory.GetSiteContext(queryString);
                Assert.IsNotNull(siteContext, string.Concat("Site from query string was not found: ", queryString));
                return siteContext;
            }
            if (this.EnableSiteConfigFiles)
            {
                string str = this.ExtractSiteConfigPathForRequestedDirectory(args);
                if (!string.IsNullOrEmpty(str))
                {
                    siteContext = this.SiteContextFactory.GetSiteContextFromFile(str);
                    Assert.IsNotNull(siteContext, string.Concat("Site from site.config was not found: ", str));
                    return siteContext;
                }
            }
            foreach (SiteInfo info in this.SiteContextFactory.GetSites())
            {
                var originalHost = args.HttpContext.Request.Headers["X-ORIGINAL-HOST"];
                if (!string.IsNullOrEmpty(originalHost) && info.HostName.Contains(originalHost))
                {
                    siteContext = new SiteContext(info);
                }
            }
            if (siteContext == null)
            {
                Uri requestUrl = args.RequestUrl;
                siteContext = this.SiteContextFactory.GetSiteContext(requestUrl.Host, args.Url.FilePath, requestUrl.Port);
            }
            return siteContext;
        }

    }
}
