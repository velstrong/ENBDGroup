using ENBDGroup.Feature.Enbd.SitecoreWebApi.Models;
using ENBDGroup.Feature.Enbd.SitecoreWebApi.ViewModels;

namespace ENBDGroup.Feature.Enbd.SitecoreWebApi.Factories
{
    public interface IHeroViewModelFactory
    {
        HeroViewModel CreateHeroViewModel(IHero heroItemDataSource, bool isExperienceEditor);
    }
}
