using Sitecore.Data.Items;
using System.Collections.Generic;

namespace ENBDGroup.Foundation.Common.Search.Models
{
    public class SearchCollection
    {
        public SearchCollection()
            : base()
        {
        }

        public int TotalRows { get; set; }

        public IEnumerable<SearchFacetCategory> Facets { get; set; }

        public IEnumerable<Item> Items { get; set; }
    }

    public class SearchFacetCategory
    {
        public string DisplayName { get; set; }

        public string Name { get; set; }

        public List<SearchFacetValue> Values { get; set; }
    }

    public class SearchFacetValue
    {
        public int Count { get; set; }

        public string Name { get; set; }
    }
}