using Helixbase.Feature.Hero.Models;

namespace Helixbase.Feature.Hero.Services
{
    public interface IHeroService
    {
        IHero GetHeroItems();
        bool IsExperienceEditor { get; }
    }
}
