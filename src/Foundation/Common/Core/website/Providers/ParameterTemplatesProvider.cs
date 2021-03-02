using ENBDGroup.Foundation.Common.Core.Models;
using Sitecore.Data.Items;
using Sitecore.Mvc.Presentation;
using Sitecore.Web;
using System.Linq;

namespace ENBDGroup.Foundation.Common.Core.Providers
{
    public class ParameterTemplatesProvider : IParameterTemplatesProvider
    {
        /// <summary>
        /// Get raw string value of a given parameter
        /// Scope: will first look at the placeholder-specific, then the rendering (setup at rendering property window)
        /// </summary>
        /// <param name="context"></param>
        /// <param name="parameter">Parameter name</param>
        /// <returns></returns>
        public string GetParameter(DynamicPlaceholderRenderContext context, string parameter)
        {
            return context.Parameters.ContainsKey(parameter) ? context.Parameters[parameter] :
                context.Rendering.Parameters.Contains(parameter) ? context.Rendering.Parameters[parameter] :
                string.Empty;
        }

        /// <summary>
        /// Get strong-typed value of a given parameter
        /// Scope: will first look at the placeholder-specific, then the rendering (setup at rendering property window)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="context"></param>
        /// <param name="parameter">Parameter name</param>
        /// <returns></returns>
        public T GetParameter<T>(DynamicPlaceholderRenderContext context, string parameter)
        {
            var rawVal = GetParameter(context, parameter);
            return GetParameterValue<T>(rawVal);
        }

        /// <summary>
        /// Get raw string value of a given parameter
        /// Scope: will always look in a RenderingItem (no fallback)
        /// </summary>
        /// <param name="rendering"></param>
        /// <param name="parameter">Parameter name</param>
        /// <returns></returns>
        public string GetParameter(RenderingItem rendering, string parameter)
        {                 
            var parameters = WebUtil.ParseUrlParameters(rendering.InnerItem["parameters"]);
            return parameters.AllKeys.Contains(parameter) ? parameters[parameter] : string.Empty;
        }

        /// <summary>
        /// Get strong-typed value of a given parameter
        /// Scope: will always look in a RenderingItem (no fallback)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="rendering"></param>
        /// <param name="parameter">Parameter name</param>
        /// <returns></returns>
        public T GetParameter<T>(RenderingItem rendering, string parameter)
        {
            var rawVal = GetParameter(rendering, parameter);
            return GetParameterValue<T>(rawVal);
        }

        /// <summary>
        /// Does the translation of a raw string value into an specific type
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="rawValue"></param>
        /// <returns></returns>
        private static T GetParameterValue<T>(string rawValue)
        {
            // Boolean
            if (typeof(bool) == typeof(T))
            {
                var parsedVal = !string.IsNullOrEmpty(rawValue);
                return (T)(object) parsedVal;
            }
            // Int?
            if (typeof(int?) == typeof(T))
            {
                int? finalVal = null;
                int parsedVal;
                if (int.TryParse(rawValue, out parsedVal))
                    finalVal = parsedVal;
                return (T)(object) finalVal;
            }
            // Int
            if (typeof(int) == typeof(T))
            {
                int parsedVal;
                int.TryParse(rawValue, out parsedVal);
                return (T)(object) parsedVal;
            }
            // HtmlTag
            //if (typeof(HtmlTag) == typeof(T))
            //{
            //    var item = Sitecore.Context.Database.GetItem(rawValue);
            //    return item == null ? (T) (object) null : (T) (object) new HtmlTag(item);
            //}
            // String
            if (typeof(string) == typeof(T))
                return (T) (object)rawValue;

            // Other types?
            // Fallback - null
            return (T)(object)null;
        }
    }
}