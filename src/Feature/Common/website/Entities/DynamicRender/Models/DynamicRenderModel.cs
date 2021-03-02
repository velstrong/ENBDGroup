using Sitecore.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ENBDGroup.Feature.Common.Website.Entities.DynamicRender.Models
{
    public class DynamicRenderModel
    {

    }
    public class HtmlTagItem : BaseTag
    {
        public string Tag { get; set; }
    }
    public class BaseTag
    {
        public string FieldValue { get; set; }
        public string CssClass { get; set; }

        public string ContentId { get; set; }
    }
    public static class DynamicRenderConstant
    {
        public static ID HtmlTagTemplateId = ID.Parse("{327F6E98-FE3F-4BAC-A857-6A73438313CD}");
        public static ID ImageTagTemplateId = ID.Parse("{A46AD940-883D-4023-9E3A-19AA428CB23F}");
        public static ID LinkTagTemplateId = ID.Parse("{C16730FC-AB19-48A7-92E6-DD7B092EF8F6}");
    }
}