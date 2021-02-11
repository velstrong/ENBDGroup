using ENBDGroup.Foundation.Common.Content.Repositories;
using ENBDGroup.Foundation.Common.Search.BaseSearch;
using LivApp.Foundation.Content.Services;
using Microsoft.Extensions.DependencyInjection;
using Sitecore.DependencyInjection;
using System;

namespace ENBDGroup.Foundation.Common.Content.DI
{
    public class RegisterContainer : IServicesConfigurator
    {
        public void Configure(IServiceCollection serviceCollection)
        {
            serviceCollection.AddTransient<IContentRepository, ContentRepository>();

            // Allow IContentRepository to be resolved on-demand by singletons
            serviceCollection.AddSingleton<Func<IContentRepository>>(_ => () => ServiceLocator.ServiceProvider.GetService<IContentRepository>());

            serviceCollection.AddTransient<IRenderingRepository, RenderingRepository>();

            serviceCollection.AddTransient<IContextRepository, ContextRepository>();

            // Allow IContextRepository to be resolved on-demand by singletons
            serviceCollection.AddSingleton<Func<IContextRepository>>(_ => () => ServiceLocator.ServiceProvider.GetService<IContextRepository>());

            serviceCollection.AddTransient<IBaseService, BaseService>();
            serviceCollection.AddTransient<IBaseSearchService, BaseSearchService>();
        }
    }
}