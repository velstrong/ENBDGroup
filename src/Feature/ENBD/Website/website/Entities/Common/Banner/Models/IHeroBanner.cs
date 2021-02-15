using ENBDGroup.Foundation.Common.ORM.Models;
using Glass.Mapper.Sc.Configuration.Attributes;
using Glass.Mapper.Sc.Fields;

namespace ENBDGroup.Feature.Enbd.Website.Banner.Models
{
    public interface IHeroBanner : IGlassBase
    {
       
        [SitecoreField(FieldName = "Banner Title")]
        string BannerTitle { get; set; }
        [SitecoreField(FieldName = "Banner Description")]
        string BannerDescription { get; set; }
        [SitecoreField(FieldName = "Banner Image")]
        Image BannerImage { get; set; }
    }
}