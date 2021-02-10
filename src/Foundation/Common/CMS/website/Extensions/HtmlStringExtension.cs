﻿using HtmlAgilityPack;
using System.Web;

namespace LivApp.Foundation.CMS.Extensions
{
    public static class HtmlStringExtension
    {
        /// <summary>
        /// Converts img tag generated by Glass Mapper to img 
        /// </summary>
        /// <param name="Input"></param>
        /// <param name="DefaultImage"></param>
        /// <returns></returns>
        public static HtmlString ToLazyImage(this HtmlString Input, string DefaultImage = "", string CssClass = "lazy-image")
        {
            if (!Sitecore.Context.PageMode.IsExperienceEditor)
            {
                var node = HtmlNode.CreateNode(Input.ToHtmlString());

                if (node != null)
                {
                    string imageSrc = string.Empty;

                    foreach (var attr in node.Attributes)
                    {
                        if (attr.Name == "src")
                        {
                            imageSrc = attr.Value;
                            attr.Value = DefaultImage;

                            break;
                        }
                    }

                    if (!string.IsNullOrEmpty(imageSrc))
                    {
                        node.Attributes.Add("data-src", imageSrc);
                    }

                    if (node.Attributes.Contains("class"))
                    {
                        node.Attributes["class"].Value += " " + CssClass;
                    }
                    else
                    {
                        node.Attributes.Add("class", CssClass);
                    }

                    return new HtmlString(node.OuterHtml);
                }
            }

            return Input;
        }
    }
}