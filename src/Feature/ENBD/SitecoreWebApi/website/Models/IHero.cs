using System.Collections.Generic;
using Glass.Mapper.Sc.Fields;

namespace ENBDGroup.Feature.Enbd.SitecoreWebApi.Models
{
    public interface IHero : IHeroGlassBase
    {
        string HeroTitle { get; set; }
        IEnumerable<Image> HeroImages { get; set; }
    }
}