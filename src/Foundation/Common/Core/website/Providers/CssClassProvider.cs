using System.Collections.Generic;
using System.Linq;
using Sitecore.Mvc.Presentation;
using Sitecore.Text;

namespace ENBDGroup.Foundation.Common.Core.Providers
{
    public class CssClassRepository : ICssClassProvider
    {
        private readonly IParameterTemplatesProvider _parameterTemplatesProvider = new ParameterTemplatesProvider();

        public List<string> GetCssClasses(string rawValue)
        {
            var listString = new ListString(rawValue).Where(p=>!string.IsNullOrEmpty(p));
            var cssClasses = listString.Select(GetCssClass).Where(p => p != null).ToList();
            return cssClasses;
        }

        public string GetCssClass(string pathOrId)
        {
            var item = Sitecore.Context.Item.Database.GetItem(pathOrId);
            return item == null ? string.Empty : item.Fields["Css Class"].Value;
        }

        public string GetInlineCssClasses(DynamicPlaceholderRenderContext context, string param)
        {
            var rawCssClassesIds = _parameterTemplatesProvider.GetParameter(context, param);
            var cssClassesItems = GetCssClasses(rawCssClassesIds);
            return string.Join(" ", cssClassesItems.Select(p => p));
        }
    }
}