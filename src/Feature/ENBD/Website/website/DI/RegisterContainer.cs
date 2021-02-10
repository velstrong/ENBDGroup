using ENBDGroup.Feature.Liv.Website.Factories;
using ENBDGroup.Feature.Liv.Website.Mediators;
using ENBDGroup.Feature.Liv.Website.Services;
using Microsoft.Extensions.DependencyInjection;
using Sitecore.DependencyInjection;

namespace ENBDGroup.Feature.Liv.Website.DI
{
    public class RegisterContainer : IServicesConfigurator
    {
        public void Configure(IServiceCollection serviceCollection)
        {
            serviceCollection.AddTransient<IHeroMediator, HeroMediator>();
            serviceCollection.AddTransient<IHeroService, HeroService>();
            serviceCollection.AddTransient<IHeroViewModelFactory, HeroViewModelFactory>();
        }
    }
}