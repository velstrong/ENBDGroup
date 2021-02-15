using ENBDGroup.Foundation.Common.DI.Extensions;
using Microsoft.Extensions.DependencyInjection;
using Sitecore.DependencyInjection;

namespace ENBDGroup.Foundation.Common.DI
{
    public class RegisterControllers : IServicesConfigurator
    {
        public void Configure(IServiceCollection serviceCollection)
        {
            serviceCollection.AddMvcControllers(
                "ENBDGroup.Feature.*");
        }
    }
}