using ENBDGroup.Feature.Enbd.SitecoreWebApi.Factories;
using ENBDGroup.Feature.Enbd.SitecoreWebApi.Mediators;
using ENBDGroup.Feature.Enbd.SitecoreWebApi.Services;
using Microsoft.Extensions.DependencyInjection;
using Sitecore.DependencyInjection;

namespace ENBDGroup.Feature.Enbd.SitecoreWebApi.DI
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