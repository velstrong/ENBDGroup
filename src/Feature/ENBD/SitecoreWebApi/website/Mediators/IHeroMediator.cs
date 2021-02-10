using ENBDGroup.Feature.Enbd.SitecoreWebApi.ViewModels;
using ENBDGroup.Foundation.Common.Core.Models;

namespace ENBDGroup.Feature.Enbd.SitecoreWebApi.Mediators
{
    public interface IHeroMediator
    {
        MediatorResponse<HeroViewModel> RequestHeroViewModel();
    }
}
