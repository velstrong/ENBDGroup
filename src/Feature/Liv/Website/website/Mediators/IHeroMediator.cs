using Helixbase.Feature.Hero.ViewModels;
using ENBDGroup.Foundation.Common.Core.Models;

namespace Helixbase.Feature.Hero.Mediators
{
    public interface IHeroMediator
    {
        MediatorResponse<HeroViewModel> RequestHeroViewModel();
    }
}
