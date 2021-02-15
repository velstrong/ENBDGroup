using ENBDGroup.Foundation.Common.Content.Repositories;
using ENBDGroup.Foundation.Common.Logging.Repositories;
using Sitecore.Data.Items;
using System;
using System.Collections.Generic;

namespace LivApp.Foundation.Content.Services
{
    public class BaseService : IBaseService
    {
        private readonly IContextRepository _contextRepository;
        private readonly ILogRepository _logRepository;
        private readonly IRenderingRepository _renderingRepository;
        private readonly IContentRepository _contentRepository;

        public BaseService(IContextRepository contextRepository,
            ILogRepository logRepository, IRenderingRepository renderingRepository, IContentRepository contentRepository)
        {
            _contextRepository = contextRepository;
            _logRepository = logRepository;
            _renderingRepository = renderingRepository;
            _contentRepository = contentRepository;
        }

        /// <summary>
        ///     Get an item using the rendering repository
        /// </summary>
        /// <returns>The Header datasource item from the Content API</returns>
        public T GetDataSourceItem<T>() where T : class
        {
            var dataSource = _renderingRepository.GetDataSourceItem<T>();

            // Basic example of using the wrapped logger
            if (dataSource == null)
                _logRepository.Warn("The datasource is empty");

            return dataSource;
        }
        public T GetDataSourceOrCurrentItem<T>() where T : class
        {
            var dataSource = _renderingRepository.GetDataSourceItem<T>();

            // Basic example of using the wrapped logger
            if (dataSource == null)
                dataSource = _renderingRepository.GetPageContextItem<T>();

            return dataSource;
        }
        public T GetItem<T>(Guid itemId) where T : class
        {
            return _contentRepository.GetItem<T>(itemId);
        }

        public T GetItem<T>(Item item) where T : class
        {
            return _contentRepository.GetItem<T>(item);
        }
        public List<T> GetItems<T>(IEnumerable<Item> items) where T : class
        {
            var resultItems = new List<T>();
            if (items != null)
            {
                foreach (Item item in items)
                {
                    resultItems.Add(_contentRepository.GetItem<T>(item));
                }
            }
            return resultItems;
        }
        public List<T> GetItems<T>(IEnumerable<Guid> itemIds) where T : class
        {
            var resultItems = new List<T>();
            if (itemIds != null)
            {
                foreach (var item in itemIds)
                {
                    resultItems.Add(_contentRepository.GetItem<T>(item));
                }
            }
            return resultItems;
        }
        public string GetRenderingParameter(string parameter)
        {
            return _renderingRepository.GetRenderingParameters(parameter);
        }
        public int GetIntRenderingParameter(string parameter,int defaultValue=0)
        {
            string value = _renderingRepository.GetRenderingParameters(parameter);
            if (int.TryParse(value, out int convertedValue))
            {
                return convertedValue;
            }
            return defaultValue;
        }

        public T GetItemFromRenderingParameter<T>(string parameter) where T : class 
        {
            var ItemIdString = GetRenderingParameter(parameter);

            if (!string.IsNullOrEmpty(ItemIdString))
            {
                // cleanup the string
                ItemIdString = Uri.UnescapeDataString(ItemIdString);

                if (Guid.TryParse(ItemIdString, out Guid ItemId))
                {
                    return GetItem<T>(ItemId);
                }
            }
            
            return null;
        }

        public bool GetBoolRenderingParameter(string parameter)
        {
            string value = _renderingRepository.GetRenderingParameters(parameter);
            return value == "1";
        }
        public bool IsExperienceEditor => _contextRepository.IsExperienceEditor;
    }
}