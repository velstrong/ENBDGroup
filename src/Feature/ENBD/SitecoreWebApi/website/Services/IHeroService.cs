using ENBDGroup.Feature.Enbd.SitecoreWebApi.Models;

namespace ENBDGroup.Feature.Enbd.SitecoreWebApi.Services
{
    public interface IHeroService
    {
        IHero GetHeroItems();
        bool IsExperienceEditor { get; }
    }
}
