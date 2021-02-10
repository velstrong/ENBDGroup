using System.Web;
using Glass.Mapper.Sc;
using ENBDGroup.Feature.Liv.Website.Models;
using ENBDGroup.Feature.Liv.Website.ViewModels;

namespace ENBDGroup.Feature.Liv.Website.Factories
{
    public class HeroViewModelFactory : IHeroViewModelFactory
    {
        private readonly IGlassHtml _glassHtml;

        public HeroViewModelFactory(IGlassHtml glassHtml)
        {
            _glassHtml = glassHtml;
        }

        public HeroViewModel CreateHeroViewModel(IHero heroItemDataSource, bool isExperienceEditor)
        {
            return new HeroViewModel
            {
                HeroImages = heroItemDataSource.HeroImages,
                HeroTitle = new HtmlString(_glassHtml.Editable(heroItemDataSource, i => i.HeroTitle,
                    new { EnclosingTag = "h2" })),
                IsExperienceEditor = isExperienceEditor
            };
        }
    }
}