using Sitecore.Pipelines.HttpRequest;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Xml.Linq;
using Sitecore.Data.Items;
using Sitecore;
using Glass.Mapper.Sc;
using Sitecore.Links;
using System.Xml;
using System.Xml.Serialization;
using Sitecore.Data.Managers;
using ENBDGroup.Foundation.Common.Search.BaseSearch;
using Sitecore.Sites;
using Sitecore.Diagnostics;
using Sitecore.Configuration;
using Sitecore.Web;
using System.Text.RegularExpressions;

namespace ENBDGroup.Foundation.Common.CMS.Pipelines
{
    public class SitemapXmlProcessor : HttpRequestProcessor
    {
        // Default Value
        private const string ChangeFrequency = "Daily";
        private const string Priority = "0.5";

        Sitecore.Data.Database master = Sitecore.Data.Database.GetDatabase("web");

        private readonly XNamespace nsXhtml = "http://www.w3.org/1999/xhtml";
        private readonly XNamespace nsSitemap = "http://www.sitemaps.org/schemas/sitemap/0.9";

        /// <summary>
        /// Generate the Sitemap XML
        /// </summary>
        /// <param name="args"></param>
        public override void Process(HttpRequestArgs args)
        {
            try
            {
                Sitecore.Diagnostics.Assert.ArgumentNotNull(args, "args");
                Sitecore.Diagnostics.Log.Info("Start - sitemap.xml", this);

                if (!File.Exists(args.HttpContext.Request.PhysicalPath) && !Directory.Exists(args.HttpContext.Request.PhysicalPath))
                {
                    Sitecore.Diagnostics.Log.Info("Sitemap.xml if Not Exists", this);
                    if (args.Url == null || !args.Url.FilePath.ToLower().EndsWith("sitemap.xml"))
                        return;

                    List<Item> items = null;
                    
                    var sitecoreContext = new SitecoreContext(master);
                    var siteContext = GetSiteContext(HttpContext.Current.Request.Url);

                    if (siteContext != null)
                    {
                        items = new List<Item>();
                        Item homeItem = master.GetItem(siteContext.StartPath); 

                        if (homeItem.HasChildren)
                        {
                            var searchService = new BaseSearchService();

                            var results = searchService.GetAllPageItemsFromIndex(homeItem).ToList();
                            if (results != null && results.Any())
                            {
                                items.AddRange(results);

                                // Move Home item at first index position
                                var homeItemIndex = items.FindIndex(x => x.ID == homeItem.ID);
                                if (homeItemIndex > 0)
                                {
                                    var item = results[homeItemIndex];
                                    items[homeItemIndex] = items[0];
                                    items[0] = item;
                                }
                            }
                        }
                    }

                    var sitemapXDocument = new XDocument(new XDeclaration("1.0", "utf-8", "yes"));

                    var itemList = new List<Item>();
                    foreach (var item in items)
                    {
                        if (!string.IsNullOrEmpty(item["Is Sitemap XML"]))
                        {
                            if (item["Is Sitemap XML"] == "1")
                                itemList.Add(item);
                        }
                    }

                    items = itemList;
                    if (items != null && items.Any())
                    {
                        UrlOptions urlOptions = LinkManager.GetDefaultUrlOptions();
                        urlOptions.AlwaysIncludeServerUrl = true;
                        urlOptions.LanguageEmbedding = LanguageEmbedding.Always;

                        sitemapXDocument.Add(GetSitemapUrlset(items, sitecoreContext, urlOptions));
                    }

                    XmlDocument xmlDocument = ConvertXDocumentToXmlDocument(sitemapXDocument);

                    var response = HttpContext.Current.Response;
                    response.ContentType = "text/xml";
                    XmlSerializer xmlSerializer = new XmlSerializer(typeof(XmlDocument));
                    xmlSerializer.Serialize(response.OutputStream, xmlDocument);
                    response.End();
                }
            }
            catch (System.Threading.ThreadAbortException)
            {
                // Ignore it
            }
            catch (Exception ex)
            {
                Sitecore.Diagnostics.Log.Error("Error - sitemap.xml", ex, this);
            }
        }

        #region Private Methods
        private static SiteContext GetSiteContext(Uri requestUrl)
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
        private XElement GetSitemapUrlset(List<Item> items, ISitecoreContext sitecoreContext, UrlOptions urlOptions)
        {
            var urlSet = new XElement(nsSitemap + "urlset",
                                      new XAttribute("xmlns", nsSitemap),
                                      new XAttribute(XNamespace.Xmlns + "xhtml", nsXhtml),
                                      items.Select(x => GetXmlNode(x, sitecoreContext, urlOptions)));

            return urlSet;
        }

        private XElement GetXmlNode(Item item, ISitecoreContext sitecoreContext, UrlOptions urlOptions)
        {
            XElement urlNode = null;

            // Set Change Frequency & Priority
            string changeFreq = ChangeFrequency;
            string priority = Priority;
            var siteContext = GetSiteContext(HttpContext.Current.Request.Url);
            var rootItem = master.GetItem(siteContext.RootPath);  //Context.Site.Database.GetItem(Context.Site.RootPath);

            if (rootItem != null)
            {
                changeFreq = !string.IsNullOrEmpty(rootItem["ChangeFrequency"]) ? rootItem["ChangeFrequency"] : ChangeFrequency;
                priority = !string.IsNullOrEmpty(rootItem["Priority"]) ? rootItem["Priority"] : ChangeFrequency;
            }

            urlNode = new XElement(nsSitemap + "url",
                      new XElement(nsSitemap + "loc", LinkManager.GetItemUrl(item, urlOptions)),
                      new XElement(nsSitemap + "lastmod", item.Statistics.Updated.ToString("yyyy-MM-dd")),
                      new XElement(nsSitemap + "changefreq", changeFreq),
                      new XElement(nsSitemap + "priority", priority)
            );

            XElement linkNode = null;
            foreach (var language in item.Languages)
            {
                var versionCount = ItemManager.GetVersions(item, language).Count;
                if (versionCount == 0)
                    continue;

                urlOptions.Language = language;
                linkNode = new XElement(nsXhtml + "link",
                                        new XAttribute("rel", "alternate"),
                                        new XAttribute("hreflang", language.Name),
                                        new XAttribute("href", LinkManager.GetItemUrl(item, urlOptions)));
                urlNode.Add(linkNode);
            }

            return urlNode;
        }

        private XmlDocument ConvertXDocumentToXmlDocument(XDocument xDocument)
        {
            var xmlDocument = new XmlDocument();
            using (var xmlReader = xDocument.CreateReader())
            {
                xmlDocument.Load(xmlReader);
            }
            return xmlDocument;
        }
        #endregion
    }
}