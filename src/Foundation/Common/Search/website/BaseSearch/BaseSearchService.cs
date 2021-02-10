using ENBDGroup.Foundation.Common.Search.Models;
using Sitecore;
using Sitecore.ContentSearch;
using Sitecore.ContentSearch.Linq;
using Sitecore.ContentSearch.Linq.Utilities;
using Sitecore.ContentSearch.SearchTypes;
using Sitecore.ContentSearch.Security;
using Sitecore.Data;
using Sitecore.Data.Items;
using Sitecore.Globalization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace ENBDGroup.Foundation.Common.Search.BaseSearch
{
    public class BaseSearchService : IBaseSearchService
    {
        public bool ForceMasterIndex { get; set; }

        public string IndexName
        {
            get
            {
                return GetIndexName(ForceMasterIndex);
            }
        }
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
        public SearchCollection QueryItems<TSearchType>(IEnumerable<ID> parentFolderIds, IEnumerable<ID> includeTemplateIds, IEnumerable<ID> excludeTemplateIds = null,
            IEnumerable<Expression<Func<TSearchType, bool>>> filters = null, Expression<Func<TSearchType, object>> order = null, int currentPage = 0, int pageSize = 0,
            bool orderByDescending = false, bool ignoreDataFolder = true, Expression<Func<TSearchType, object>> order2 = null, bool order2ByDescending = false)
            where TSearchType : BaseSearchResultItem
        {
            SearchCollection results;

            using (IProviderSearchContext searchContext = ContentSearchManager.GetIndex(IndexName).CreateSearchContext(SearchSecurityOptions.EnableSecurityCheck))
            {

                // Reference: https://www.geekhive.com/buzz/post/2017/08/predicate-builder-advanced-search-queries/
                IQueryable<TSearchType> baseQuery = searchContext.GetQueryable<TSearchType>();
                Expression<Func<TSearchType, bool>> parentFolderPredicate = null;
                Expression<Func<TSearchType, bool>> includeTemplatePredicate = null;
                Expression<Func<TSearchType, bool>> excludeTemplatePredicate = null;

                parentFolderPredicate = ConvertParentFolderIdListToPredicate<TSearchType>(parentFolderIds, ignoreDataFolder);
                includeTemplatePredicate = ConvertIncludeTemplateIdListToPredicate<TSearchType>(includeTemplateIds);
                excludeTemplatePredicate = ConvertExcludeTemplateIdListToPredicate<TSearchType>(excludeTemplateIds);

                baseQuery = baseQuery.Where(x => x.Language == Context.Language.Name);

                if (parentFolderPredicate != null)
                {
                    baseQuery = baseQuery.Filter(parentFolderPredicate);
                }

                if (includeTemplatePredicate != null)
                {
                    baseQuery = baseQuery.Filter(includeTemplatePredicate);
                }

                if (excludeTemplatePredicate != null)
                {
                    baseQuery = baseQuery.Filter(excludeTemplatePredicate);
                }

                if (filters != null && filters.Any())
                {
                    foreach (Expression<Func<TSearchType, bool>> filter in filters)
                    {
                        baseQuery = baseQuery.Where(filter);
                    }
                }

                if (currentPage > 0 && pageSize > 0)
                {
                    baseQuery = baseQuery.Page(currentPage - 1, pageSize);
                }

                // Set Order
                if (order2 != null && order != null)
                {
                    if (orderByDescending && order2ByDescending)
                    {
                        baseQuery = baseQuery.OrderByDescending(order).ThenByDescending(order2);
                    }
                    else if (orderByDescending && !order2ByDescending)
                    {
                        baseQuery = baseQuery.OrderByDescending(order).ThenBy(order2);
                    }
                    else if (!orderByDescending && order2ByDescending)
                    {
                        baseQuery = baseQuery.OrderBy(order).ThenByDescending(order2);
                    }
                    else
                        baseQuery = baseQuery.OrderBy(order).ThenBy(order2);
                }
                else if (order != null)
                {
                    if (orderByDescending)
                        baseQuery = baseQuery.OrderByDescending(order);
                    else
                        baseQuery = baseQuery.OrderBy(order);
                }
                // TODO: Facets

                // Check for Latest Version: https://briancaos.wordpress.com/2017/05/05/sitecore-contentsearch-get-latest-version/
                baseQuery = baseQuery.Where(x => x["_latestversion"].Equals("1"));

                SearchResults<TSearchType> searchResults = baseQuery.GetResults<TSearchType>();
                if (searchResults != null)
                {
                    results = new SearchCollection()
                    {
                        TotalRows = searchResults.TotalSearchResults,
                    };

                    var scoreValue = searchResults.Select(item => item.Score);

                    var searchItems = searchResults.Where(x => x.Document.GetItem() != null)
                                                   .Select(x => x.Document.GetItem());
                    results.Items = searchItems;
                    return results;
                }

                return null;
            }
        }
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
        public IEnumerable<Item> GetAllPageItemsFromIndex(Item searchRootItem)
        {
            var pathsQuery = PredicateBuilder.True<SearchResultItem>();
            pathsQuery = pathsQuery.And(x => x.Paths.Contains(searchRootItem.ID))
                                   .And(x => !x.Path.Contains("/data"));

            var baseQuery = PredicateBuilder.True<SearchResultItem>();
            baseQuery = baseQuery.And(pathsQuery);

            using (var searchContext = ContentSearchManager.GetIndex(Indexes.Web).CreateSearchContext())
            {
                var searchResults = searchContext.GetQueryable<SearchResultItem>()
                                                 .Where(baseQuery)
                                                 .GetResults()
                                                 .AsEnumerable()
                                                 .Where(x => x.Document.GetItem() != null)
                                                 .Select(x => x.Document.GetItem());

                return searchResults;
            }
        }

        public IEnumerable<Item> FastQueryItems(List<string> itemTemplateIDs, string itemPath)
        {
            StringBuilder startItemPath = new StringBuilder(@"/");

            Sitecore.Data.Database db = Sitecore.Context.Database;

            if (string.IsNullOrEmpty(itemPath))
            {
                return new List<Item>();
            }
            startItemPath.Append(string.Join("/", itemPath.Split(new char[] { '/' },
                StringSplitOptions.RemoveEmptyEntries).Select(x => string.Format("#{0}#", x))));
            //create fast query
            System.Text.StringBuilder sbFastQuery = new System.Text.StringBuilder(string.Format("fast:{0}//*", startItemPath));

            if (itemTemplateIDs != null)
            {
                sbFastQuery.Append("[");
                if (itemTemplateIDs != null)
                {
                    sbFastQuery.Append("(");
                    for (int i = 0; i < itemTemplateIDs.Count; i++)
                    {
                        if (i != 0)
                        {
                            sbFastQuery.Append(" or ");
                        }
                        sbFastQuery.AppendFormat(@"@@templateid='{0}'", itemTemplateIDs[i]);
                    }
                    sbFastQuery.Append(")");
                }

                sbFastQuery.Append("]");
            }

            IEnumerable<Item> itemsResult = db.SelectItems(sbFastQuery.ToString());

            itemsResult = itemsResult.OrderBy(x => x.Appearance.Sortorder)
                .ThenBy(x => x.Name).ToList();
            return itemsResult;

        }
        private bool HasLanguageVersion(Item scItem, Language lang)
        {
            if (scItem == null || lang == null)
                return false;

            return scItem.Versions.Count > 0 && scItem.Versions.GetLatestVersion(lang).Versions.Count > 0;
        }

        #region Private Methods
        private Expression<Func<T, bool>> ConvertParentFolderIdListToPredicate<T>(IEnumerable<ID> parentFolderIds, bool ignoreDataFolder = true)
            where T : BaseSearchResultItem
        {
            if (parentFolderIds == null || !parentFolderIds.Any())
                return null;

            var predicate = PredicateBuilder.True<T>();
            foreach (ID id in parentFolderIds)
            {
                predicate = predicate.Or(x => x.Paths.Contains(id));
            }

            if (ignoreDataFolder)
                predicate = predicate.And(x => !x.Path.Contains("/data"));

            return predicate;
        }

        private Expression<Func<T, bool>> ConvertIncludeTemplateIdListToPredicate<T>(IEnumerable<ID> templateIds)
            where T : BaseSearchResultItem
        {
            if (templateIds == null || !templateIds.Any())
                return null;

            var predicate = PredicateBuilder.True<T>();
            foreach (ID id in templateIds)
            {
                predicate = predicate.Or(x => x.TemplateId == id);
            }

            return predicate;
        }

        private Expression<Func<T, bool>> ConvertExcludeTemplateIdListToPredicate<T>(IEnumerable<ID> templateIds)
           where T : BaseSearchResultItem
        {
            if (templateIds == null || !templateIds.Any())
                return null;

            var predicate = PredicateBuilder.True<T>();
            foreach (ID id in templateIds)
            {
                predicate = predicate.And(x => x.TemplateId != id);
            }

            return predicate;
        }

        /// <summary>
        /// Get the search index name
        /// </summary>
        /// <param name="forceMasterIndex"></param>
        /// <returns>string</returns>
        public static string GetIndexName(bool forceMasterIndex)
        {
            if (forceMasterIndex)
                return Indexes.Master;

            string indexName = Indexes.Master;
            if (Context.PageMode.IsNormal || Context.PageMode.IsDebugging)
            {
                indexName = Indexes.Web;
            }
            return indexName;
        }
        #endregion
    }

}