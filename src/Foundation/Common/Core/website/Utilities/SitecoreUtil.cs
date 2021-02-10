using ENBDGroup.Foundation.Common.Core.Models;
using Sitecore.Data.Items;
using Sitecore.Diagnostics;
using Sitecore.Globalization;
using Sitecore.Links;
using Sitecore.Resources.Media;
using Sitecore.SecurityModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ENBDGroup.Foundation.Common.Core.Utilities
{
    public static class SitecoreUtil
    {
        public static string GetMediaUrlWithServer(MediaItem mediaItem, Item item = null)
        {
            item = item ?? Sitecore.Context.Item;
            var mediaOptions = new MediaUrlOptions { AbsolutePath = true };
            var mediaUrl = MediaManager.GetMediaUrl(mediaItem, mediaOptions);
            return mediaUrl;
        }
        public static string GetItemUrlWithServer(Item item)
        {
            var options = new UrlOptions { AlwaysIncludeServerUrl = false, LanguageEmbedding = LanguageEmbedding.Always, AddAspxExtension = false };
            var originalHost = System.Web.HttpContext.Current.Request.Headers["X-ORIGINAL-HOST"];
            string itemUrl = string.Empty;
            if (!string.IsNullOrEmpty(originalHost))
            {
                itemUrl = string.Format("{0}://{1}{2}", System.Web.HttpContext.Current.Request.Headers["X-FORWARDED-PROTO"],
                    originalHost, LinkManager.GetItemUrl(item, options));
            }
            else
            {
                options.AlwaysIncludeServerUrl = true;
                itemUrl = LinkManager.GetItemUrl(item, options);
            }
            return itemUrl;
        }
        public static string GetItemUrl(Item item, string language)
        {
            using (new LanguageSwitcher(language))
            {
                var options = new UrlOptions { LanguageEmbedding = LanguageEmbedding.Always, AddAspxExtension = false };
                var itemUrl = LinkManager.GetItemUrl(item, options);
                return itemUrl;
            }
        }
        public static string GetItemUrl(Item item)
        {
            var options = new UrlOptions { LanguageEmbedding = LanguageEmbedding.Always, AddAspxExtension = false };
            var itemUrl = LinkManager.GetItemUrl(item, options);
            return itemUrl;
        }
        public static string GetItemUrl(Guid guid)
        {
            if (guid != Guid.Empty)
            {
                var item = Sitecore.Context.Database.GetItem(Sitecore.Data.ID.Parse(guid));
                var options = new UrlOptions { LanguageEmbedding = LanguageEmbedding.Always, AddAspxExtension = false };
                var itemUrl = LinkManager.GetItemUrl(item, options);
                return itemUrl;
            }
            return string.Empty;
        }
        public static Item GetItem(string itemId)
        {
            if (!string.IsNullOrEmpty(itemId))
            {
                return Sitecore.Context.Database.GetItem(Sitecore.Data.ID.Parse(itemId));
            }
            return null;
        }
        public static Item GetItem(Guid guid)
        {
            if (guid != Guid.Empty)
            {
                return Sitecore.Context.Database.GetItem(Sitecore.Data.ID.Parse(guid));
            }
            return null;
        }
        public static Item GetItemByPath(string path, string database = "")
        {
            if (string.IsNullOrEmpty(database))
            {
                return Sitecore.Context.Database.SelectSingleItem(path);
            }
            else
            {
                return Sitecore.Configuration.Factory.GetDatabase(database).SelectSingleItem(path);
            }
        }

        public static string GetUserIp()
        {
            var forwarded = HttpContext.Current.Request.Headers["X-Forwarded-For"];
            if (!string.IsNullOrEmpty(forwarded))
            {
                string[] forwardedSplit = forwarded.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                if (forwardedSplit.Length > 0)
                {
                    string clientIp = forwardedSplit[0];

                    string[] ip = forwarded.Split(new char[] { ':' }, StringSplitOptions.RemoveEmptyEntries);
                    if (ip.Length > 0)
                    {
                        return ip[0];
                    }
                }
            }
            return string.Empty;
        }
        public static bool HasArabicGlyphs(string text)
        {
            char[] glyphs = text.ToCharArray();
            foreach (char glyph in glyphs)
            {
                if (glyph >= 0x600 && glyph <= 0x6ff) return true;
                if (glyph >= 0x750 && glyph <= 0x77f) return true;
                if (glyph >= 0xfb50 && glyph <= 0xfc3f) return true;
                if (glyph >= 0xfe70 && glyph <= 0xfefc) return true;
            }
            return false;
        }
        public static void UpdateSitecoreFields(Item newItem, string sitecoreFieldName, string sitecoreFieldType,
             string sitecoreValue)

        {
            using (new SecurityDisabler())

            {
                newItem.Editing.BeginEdit();
                try
                {
                    if (!string.IsNullOrWhiteSpace(sitecoreFieldName) &&
                        newItem.Fields[sitecoreFieldName] != null)
                    {
                        if (sitecoreFieldType == Constants.FieldIds.MultiLineTextFieldId || sitecoreFieldType == Constants.FieldIds.RichTextFieldId ||
                            sitecoreFieldType == Constants.FieldIds.SingleLineFieldId || sitecoreFieldType == Constants.FieldIds.DropLinkFieldId ||
                            sitecoreFieldType == Constants.FieldIds.DropListFieldId)
                        {
                            new UtilityMethods().SetSimpleField(newItem, sitecoreFieldName, sitecoreValue);
                        }
                        else if (sitecoreFieldType == Constants.FieldIds.CheckboxFieldId && sitecoreValue.ToLower() == "true")
                        {
                            new UtilityMethods().SetCheckboxField(newItem, sitecoreFieldName);
                        }
                        else if (sitecoreFieldType == Constants.FieldIds.DropListFieldId || sitecoreFieldType == Constants.FieldIds.MultiListFieldId ||
                                 sitecoreFieldType == Constants.FieldIds.TreeListFieldId || sitecoreFieldType == Constants.FieldIds.TreeListExFieldId)
                        {
                            new UtilityMethods().AddToMultiListField(newItem, sitecoreFieldName, sitecoreValue);
                        }
                        else if (sitecoreFieldType == Constants.FieldIds.GeneralLinkFieldId)
                        {
                            new UtilityMethods().SetLinkField(newItem, sitecoreFieldName, sitecoreValue);
                        }
                        else
                        {
                            new UtilityMethods().SetSimpleField(newItem, sitecoreFieldName, sitecoreValue);
                        }
                    }
                    newItem.Editing.EndEdit();
                }

                catch (Exception ex)
                {
                    Log.Error("Could not update item " + newItem.Paths.FullPath + ": " + ex.Message, typeof(SitecoreUtil));
                    newItem.Editing.CancelEdit();
                }
            }

        }
    }
}