using ENBDGroup.Foundation.Common.ORM.Models;
using Glass.Mapper.Sc.Configuration.Attributes;
using Glass.Mapper.Sc.Fields;

namespace ENBDGroup.Feature.Enbd.Website.Banner.Models
{
    public interface IYoutubeVideoBanner : IGlassBase
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
        [SitecoreField(FieldName = "Youtube Video Id")]
        string YoutubeVideoId { get; set; }
        
    }
}