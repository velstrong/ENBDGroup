using ENBDGroup.Feature.Liv.Website.Models;
using ENBDGroup.Foundation.Common.Search.Models;

namespace ENBDGroup.Feature.Liv.Website.Services
{
    public interface IHeroService
    {
        IHero GetHeroItems();
        bool IsExperienceEditor { get; }
    }
}
