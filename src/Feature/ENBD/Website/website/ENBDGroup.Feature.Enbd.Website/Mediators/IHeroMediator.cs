using ENBDGroup.Feature.Liv.Website.ViewModels;
using ENBDGroup.Foundation.Common.Core.Models;

namespace ENBDGroup.Feature.Liv.Website.Mediators
{
    public interface IHeroMediator
    {
        MediatorResponse<HeroViewModel> RequestHeroViewModel();
    }
}
