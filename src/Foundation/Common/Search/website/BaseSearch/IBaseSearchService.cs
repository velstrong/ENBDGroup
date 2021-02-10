using ENBDGroup.Foundation.Common.Search.Models;
using Sitecore.Data;
using Sitecore.Data.Items;
using Sitecore.Globalization;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace ENBDGroup.Foundation.Common.Search.BaseSearch
{
    public interface IBaseSearchService
    {
        /// <summary>
        /// Search the Items
        /// </summary>
        /// <typeparam name="TItem"></typeparam>
        /// <typeparam name="TSearchType"></typeparam>
        /// <param name="parentFolderIds"></param>
        /// <param name="includeTemplateIds"></param>
        /// <param name="excludeTemplateIds"></param>
        /// <param name="filters"></param>
        /// <param name="order"></param>
        /// <param name="currentPage"></param>
        /// <param name="pageSize"></param>
        /// <param name="orderByDescending"></param>
        /// <param name="ignoreDataFolder"></param>
        /// <param name="order2"></param>
        /// <param name="order2ByDescending"></param>
        /// <returns>list of result items</returns>
         SearchCollection QueryItems<TSearchType>(IEnumerable<ID> parentFolderIds, IEnumerable<ID> includeTemplateIds,
             IEnumerable<ID> excludeTemplateIds = null,
            IEnumerable<Expression<Func<TSearchType, bool>>> filters = null,
            Expression<Func<TSearchType, object>> order = null, int currentPage = 0, int pageSize = 0,
            bool orderByDescending = false, bool ignoreDataFolder = true,
            Expression<Func<TSearchType, object>> order2 = null, bool order2ByDescending = false)
           where TSearchType : BaseSearchResultItem;
        /// <summary>
        /// Gett All the Items
        /// </summary>
        /// <typeparam name="TItem"></typeparam>
        /// <typeparam name="TSearchType"></typeparam>
        /// <param name="parentFolderIds"></param>
        /// <param name="includeTemplateIds"></param>
        /// <param name="excludeTemplateIds"></param>
        /// <param name="filters"></param>
        /// <param name="order"></param>
        /// <param name="currentPage"></param>
        /// <param name="pageSize"></param>
        /// <param name="orderByDescending"></param>
        /// <param name="ignoreDataFolder"></param>
        /// <param name="order2"></param>
        /// <param name="order2ByDescending"></param>
        /// <returns>list of result items</returns>
        IEnumerable<Item> GetAllPageItemsFromIndex(Item searchRootItem);

        IEnumerable<Item> FastQueryItems(List<string> itemTemplateIDs,string itemPath);
    }
}