using Glass.Mapper.Sc.Fields;
using HtmlAgilityPack;
using System.Web;

namespace ENBDGroup.Foundation.Common.CMS.Helpers
{
    public static class RazorViewHelper
    {
        /// <summary>
        /// Gets a rendered HTML Attribute. If the content is empty, the whole attribute is not returned.
        /// </summary>
        /// <param name="Content">The content of the attribute</param>
        /// <param name="Attribute">The attribute to add. Defaults to HTML id attribute.</param>
        /// <param name="Format"></param>
        /// <returns></returns>
        public static string GetHtmlAttribute(string Content, string Attribute = "id", string Format = "{0}={1}")
        {
            return string.IsNullOrEmpty(Content) ? string.Empty : string.Format(Format, Attribute, Content);
        }

        /// <summary>
        /// Gets the background image from an image field.
        /// </summary>
        /// <param name="ImageField"></param>
        /// <param name="DefaultImage">The default image to show if the image field is not specified.</param>
        /// <returns></returns>
        public static string GetBackgroundImage(Image ImageField, string DefaultImage = null)
        {
            return ImageField == null ? DefaultImage : ImageField.Src;
        }

        /// <summary>
        /// Gets the class from a given Link field.
        /// </summary>
        /// <param name="Link"></param>
        /// <param name="Default">The class to return if the link is null.</param>
        /// <param name="Prepend">Prepend the provided default class to the class value of the link.</param>
        /// <returns></returns>
        public static string GetLinkClass(Link Link, string Default = null, bool Prepend = false)
        {
            if (Link == null) return Default;

            return (Prepend ? " " + Default : string.Empty) + (string.IsNullOrEmpty(Link.Class) ? Default : string.Empty);
        }

        /// <summary>
        /// Gets the target window of the given link
        /// </summary>
        /// <param name="Link"></param>
        /// <returns>The target HTML attribute value. Either "_blank" or "_self"</returns>
        public static string GetLinkTarget(Link Link)
        {
            return Link != null && Link.Type == LinkType.External ? "_blank" : "_self";
        }
    }
}