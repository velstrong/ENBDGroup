using Sitecore.Data.Items;
using Sitecore.Mvc.Presentation;

namespace ENBDGroup.Foundation.Common.Core.Providers
{
    public interface IParameterTemplatesProvider
    {
        string GetParameter(DynamicPlaceholderRenderContext context, string parameter);
        T GetParameter<T>(DynamicPlaceholderRenderContext context, string parameter);
        string GetParameter(RenderingItem rendering, string parameter);
        T GetParameter<T>(RenderingItem rendering, string parameter);
    }
}
