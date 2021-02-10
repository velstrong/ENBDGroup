using Sitecore.Data.Items;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ENBDGroup.Foundation.Common.Core.Utilities;
using Sitecore.Data;
using Sitecore.Data.Fields;

namespace ENBDGroup.Foundation.Common.Core.Extensions
{
    public static class ItemExtensions
    {
        public static string GetImageFieldURL(this Item source, string fieldName)
        {
            Sitecore.Data.Fields.ImageField imgField = (Sitecore.Data.Fields.ImageField)source.Fields[fieldName];
            if (imgField != null && imgField.MediaItem != null)
            {
                MediaItem image = new MediaItem(imgField.MediaItem);

                if (image != null)
                {
                    return SitecoreUtil.GetMediaUrlWithServer(image);
                }
            }
            return string.Empty;
        }
        public static string GetCustomAttributes(this Item source, string fieldName)
        {
            Sitecore.Data.Fields.LinkField linkField = (Sitecore.Data.Fields.LinkField)source.Fields[fieldName];
            if (linkField != null)
            {
                return linkField.GetAttribute("customattributes");
            }
            return string.Empty;
        }
        public static string GetFieldValue(this Item source, string fieldName)
        {
            if (source.Fields[fieldName] != null && !string.IsNullOrEmpty(source.Fields[fieldName].Value))
            {
                return source.Fields[fieldName].Value.Trim();
            }

            return string.Empty;
        }
        public static Item GetInternalLinkFieldItem(this Item item, string fieldName)
        {
            Item targetItem = null;
            var itemValue = item.GetFieldValue(fieldName);
            if (!string.IsNullOrEmpty(itemValue))
            {
                targetItem = GetItemByPath(itemValue);
            }
            return targetItem;
        }
        public static Item GetInternalLinkFieldItem(this Item item, string fieldName, Database database)
        {
            Item targetItem = null;
            var itemValue = item.GetFieldValue(fieldName);
            if (!string.IsNullOrEmpty(itemValue))
            {
                if (database != null)
                {
                    targetItem = database.SelectSingleItem(itemValue);
                }
                else
                {
                    targetItem = Sitecore.Context.Database.SelectSingleItem(itemValue);
                }
            }
            return targetItem;
        }
        public static Item GetItemByPath(string itemPath, string database = "web")
        {
            Item currentItem = null;
            Database currentDb = Database.GetDatabase(database);
            if (!string.IsNullOrEmpty(itemPath))
            {
                currentItem = currentDb.SelectSingleItem(itemPath);
            }
            return currentItem;
        }
        public static int GetNumericFieldValue(this Item source, string fieldName)
        {
            if (source.Fields[fieldName] != null && !string.IsNullOrEmpty(source.Fields[fieldName].Value))
            {
                string value = source.Fields[fieldName].Value;
                if(Int32.TryParse(value,out int result))
                {
                    return result;
                }
            }
            return 0;
        }
        public static DateTime? GetDateFieldValue(this Item item, string fieldName)
        {
            DateTime? date = null;
            var dateField = item.Fields[fieldName] == null ? null : (DateField)item.Fields[fieldName];
            if (dateField != null && dateField.DateTime != DateTime.MinValue)
            {
                date = dateField.DateTime;
            }
            return date;
        }
        public static bool HasContextLanguage(this Item item)
        {
            if (item == null || item.Versions == null || item.Versions.Count == 0)
                return false;
            var latestLanguageVersion = item.Versions.GetLatestVersion();
            return (latestLanguageVersion != null) && (latestLanguageVersion.Versions.Count > 0);
        }
    }
}