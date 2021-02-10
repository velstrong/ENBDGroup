using Sitecore;
using Sitecore.Data;
using Sitecore.Data.Items;
using Sitecore.Data.Managers;
using Sitecore.Data.Templates;
using Sitecore.Links.UrlBuilders;
using Sitecore.Resources.Media;
using Sitecore.Web;
using System.Linq;

namespace ENBDGroup.Foundation.Common.Redirect.Extensions
{
    public static class MediaItemExtensions
    {
        public static string GetMediaUrl(this MediaItem mediaItem, MediaUrlBuilderOptions options = null)
        {
            if (options == null)
            {
                options = new MediaUrlBuilderOptions();
            }
            string mediaUrl = MediaManager.GetMediaUrl(mediaItem, options);
            mediaUrl = (mediaUrl.Contains("://") ? mediaUrl : StringUtil.EnsurePrefix('/', mediaUrl));
            return mediaUrl;
        }

        public static bool IsImage(this Media mediaItem)
        {
            if (mediaItem == null)
            {
                return false;
            }
            return mediaItem.MimeType.Contains("image/");
        }
        public static bool InheritsFrom(this Item item, ID templateId)
        {
            return item.IsDerived(templateId);
        }

       
    }

    public static class TemplateExtensions
    {
        public static bool IsDerived( this Template template, ID templateId)
        {
            return template.ID == templateId || template.GetBaseTemplates().Any(baseTemplate => IsDerived(baseTemplate, templateId));
        }

        public static Item FirstChildInheritingFrom(this Item item, ID templateId)
        {
            Item item1;
            if (item != null)
            {
                item1 = item.Children.FirstOrDefault<Item>((Item i) => i.InheritsFrom(templateId));
            }
            else
            {
                item1 = null;
            }
            
            return item1;
        }
    }
    public static class ItemExtensions
    {
        public static bool IsDerived( this Item item, ID templateId)
        {
            return TemplateManager.GetTemplate(item).IsDerived(templateId);
        }

        public static SiteInfo GetSiteInfo(this Item item)
        {
            var siteInfoList = Sitecore.Configuration.Factory.GetSiteInfoList();

            return siteInfoList.FirstOrDefault(info => item.Paths.FullPath.StartsWith(info.RootPath));
        }
    }
}
