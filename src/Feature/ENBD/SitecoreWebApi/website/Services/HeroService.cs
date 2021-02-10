using System.Linq;
using ENBDGroup.Feature.Enbd.SitecoreWebApi.Models;
using ENBDGroup.Foundation.Common.Content.Repositories;
using ENBDGroup.Foundation.Common.Logging.Repositories;
using ENBDGroup.Foundation.Common.Search.Models;
using Sitecore.Data.Items;

namespace ENBDGroup.Feature.Enbd.SitecoreWebApi.Services
{
    public class HeroService : IHeroService
    {
        private readonly IContextRepository _contextRepository;
        private readonly ILogRepository _logRepository;
        private readonly IRenderingRepository _renderingRepository;

        public HeroService(IContextRepository contextRepository,
            ILogRepository logRepository, IRenderingRepository renderingRepository)
        {
            _contextRepository = contextRepository;
            _logRepository = logRepository;
            _renderingRepository = renderingRepository;
        }

        /// <summary>
        ///     Get an item using the rendering repository
        /// </summary>
        /// <returns>The Hero datasource item from the Content API</returns>
        public IHero GetHeroItems()
        {
            var dataSource = _renderingRepository.GetDataSourceItem<IHero>();

            // Basic example of using the wrapped logger
            if (dataSource == null)
                _logRepository.Warn(Logging.Error.DataSourceError);

            return dataSource;
        }

        /// <summary>
        ///     **** This method is not required/in use. It is here as an example of how to use the computed search field ****
        ///     Get an item from the index
        /// </summary>
        /// <returns>The first item based on the Hero template</returns>
    
        public bool IsExperienceEditor => _contextRepository.IsExperienceEditor;
    }
}