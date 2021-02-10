using ENBDGroup.Feature.Liv.Website.Models;
using ENBDGroup.Feature.Liv.Website.ViewModels;

namespace ENBDGroup.Feature.Liv.Website.Factories
{
    public interface IHeroViewModelFactory
    {
        HeroViewModel CreateHeroViewModel(IHero heroItemDataSource, bool isExperienceEditor);
    }
}
