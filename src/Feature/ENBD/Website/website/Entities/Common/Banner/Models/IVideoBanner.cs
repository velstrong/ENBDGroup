using ENBDGroup.Foundation.Common.ORM.Models;
using Glass.Mapper.Sc.Configuration.Attributes;
using Glass.Mapper.Sc.Fields;

namespace ENBDGroup.Feature.Enbd.Website.Banner.Models
{
    public interface IVideoBanner : IGlassBase
    {
        [SitecoreField(FieldName = "Logo Image")]
        Image LogoImage { get; set; }
        [SitecoreField(FieldName = "Logo Link")]
        Link LogoLink { get; set; }
        [SitecoreField(FieldName = "CTA Link")]
        Link CTALink { get; set; }
        string Heading { get; set; }
        [SitecoreField(FieldName = "Spinning Text")]
        string SpinningText { get; set; }
        string Description { get; set; }
        [SitecoreField(FieldName = "Banner Image")]
        Image BannerImage { get; set; }
        [SitecoreField(FieldName = "Banner Image2")]
        Image BannerImage2 { get; set; }
        [SitecoreField(FieldName = "Video Link")]
        Link VideoLink { get; set; }
        [SitecoreField(FieldName = "Placeholder Image")]
        Image PlaceholderImage { get; set; }
        
    }
}