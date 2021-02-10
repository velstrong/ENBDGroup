using Sitecore.Pipelines.HttpRequest;
using System;
using System.Collections.Generic;
using System.Linq;
using Sitecore;
using Sitecore.Data;
using Sitecore.Data.Items;
using System.Web;
using Sitecore.Web;
using System.IO;
using ENBDGroup.Foundation.Common.Core.Extensions;
using System.Net;
using Sitecore.Mvc.Pipelines;
using Sitecore.Mvc.Pipelines.Response.GetPageItem;
using Sitecore.Sites;

namespace LivApp.Foundation.CMS.Pipelines
{
    public class ItemNotFoundResolver : HttpRequestProcessor
    {
        /// <summary>
        /// Assign the 404 item as context item if item is not found
        /// </summary>
        /// <param name="args"></param>
        public override void Process(HttpRequestArgs args)
        {
            if (SiteContextNotFoundItemService.EnableNotFoundItem(Context.Site))
            {
                if (IsValidContextItemResolved()
                || args.LocalPath.StartsWith("/sitecore")
                || RequestIsForPhysicalFile(args.Url.FilePath))
                    return;
                Context.Item = GetSiteSpecificNotFoundItem();
                if (Context.Item == null)
                    return;
                ItemNotFoundStatusRepository.Set(true);
            }

        }
        protected virtual bool IsValidContextItemResolved()
        {
            if (Context.Item == null || !Context.Item.HasContextLanguage())
                return false;
            return !(Context.Item.Visualization.Layout == null
            && string.IsNullOrEmpty(WebUtil.GetQueryString("sc_layout")));
        }
        protected virtual bool RequestIsForPhysicalFile(string filePath)
        {
            return File.Exists(HttpContext.Current.Server.MapPath(filePath));
        }
        protected virtual Item GetSiteSpecificNotFoundItem()
        {
            var rootItem = Context.Site.Database.GetItem(Context.Site.RootPath);
            if (rootItem != null)
            {
                var errorItemPath = rootItem.GetFieldValue("Page Not Found Link");
                if (!string.IsNullOrEmpty(errorItemPath))
                {
                    return SiteContextNotFoundItemService.GetItemByPath(Context.Site, errorItemPath);
                }
            }
            return SiteContextNotFoundItemService.GetItemBySiteProperty(Context.Site, Constants.NotFoundItemPropertyKey);
        }
    }


    public class Constants
    {
        public const string NotFoundItemPropertyKey = "notFoundItem";
        public const string EnableNotFoundItemPropertyKey = "enableNotFound";
        public const string ResolvedFoundItemPropertyKey = "ResolvedFoundItem";
    }
    public class SiteContextNotFoundItemService
    {
        public static Item GetItemByShortPath(SiteContext siteContext, string shortPath)
        {
            shortPath = shortPath.StartsWith("/") ? shortPath.Substring(1) : shortPath;
            var fullPath = string.Concat(StringUtil.EnsurePostfix('/', siteContext.RootPath), shortPath);
            var notFoundItem = siteContext.Database.GetItem(fullPath);
            if (!notFoundItem.HasContextLanguage())
            {
                notFoundItem = siteContext.Database.GetItem(fullPath, Sitecore.Globalization.Language.Parse("en"));
            }
            return notFoundItem;
        }
        public static Item GetItemByPath(SiteContext siteContext, string path)
        {
            var notFoundItem = siteContext.Database.GetItem(path);
            if (!notFoundItem.HasContextLanguage())
            {
                notFoundItem = siteContext.Database.GetItem(path, Sitecore.Globalization.Language.Parse("en"));
            }
            return notFoundItem;
        }
        public static Item GetItemBySiteProperty(SiteContext siteContext, string propertyKey)
        {
            var property = siteContext.Properties[propertyKey];
            if (string.IsNullOrEmpty(property))
                return null;
            if (ID.IsID(property) || property.StartsWith(Sitecore.Constants.ContentPath))
                return siteContext.Database.GetItem(property);
            return GetItemByShortPath(siteContext, property);
        }
        public static bool EnableNotFoundItem(SiteContext siteContext)
        {
            return siteContext != null && !string.IsNullOrEmpty(siteContext.Properties[Constants.EnableNotFoundItemPropertyKey]) && siteContext.Properties[Constants.EnableNotFoundItemPropertyKey] == "true";
        }
    }


    public class ItemNotFoundStatusRepository
    {
        public static bool Get()
        {
            return HttpContext.Current.Items[Constants.NotFoundItemPropertyKey] != null && (bool)HttpContext.Current.Items[Constants.NotFoundItemPropertyKey];
        }
        public static void Set(bool status)
        {
            HttpContext.Current.Items[Constants.NotFoundItemPropertyKey] = status;
        }
        public static bool GetResolved()
        {
            return HttpContext.Current.Items[Constants.ResolvedFoundItemPropertyKey] != null && (bool)HttpContext.Current.Items[Constants.ResolvedFoundItemPropertyKey];
        }
        public static void SetResolved(bool status)
        {
            HttpContext.Current.Items[Constants.ResolvedFoundItemPropertyKey] = status;
        }
    }

    public class SetNotFoundStatusCode : HttpRequestProcessor
    {
        public override void Process(HttpRequestArgs args)
        {
            if (SiteContextNotFoundItemService.EnableNotFoundItem(Context.Site))
            {
                if (!ItemNotFoundStatusRepository.Get())
                    return;
                HttpContext.Current.Response.StatusCode = (int)HttpStatusCode.NotFound;
                HttpContext.Current.Response.TrySkipIisCustomErrors = true;
            }
        }
    }

    public class CheckItemResolved : MvcPipelineProcessor<GetPageItemArgs>
    {
        public override void Process(GetPageItemArgs args)
        {
            if (SiteContextNotFoundItemService.EnableNotFoundItem(Context.Site))
            {
                if (!ItemNotFoundStatusRepository.GetResolved())
                    return;
                // item has previously been resolved
                args.Result = Sitecore.Context.Item;
            }
        }
    }

}