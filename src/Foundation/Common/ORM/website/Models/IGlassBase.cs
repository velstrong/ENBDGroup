using System;
using System.Collections;
using Glass.Mapper.Sc.Configuration;
using Glass.Mapper.Sc.Configuration.Attributes;
using Sitecore.Globalization;

namespace ENBDGroup.Foundation.Common.ORM.Models
{
    public interface IGlassBase
    {
        Guid Id { get; set; }
        Language Language { get; set; }
        int Version { get; set; }
        IEnumerable BaseTemplateIds { get; set; }
        string TemplateName { get; set; }
        Guid TemplateId { get; set; }
        string Name { get; set; }
        string ContentId { get; set; }
        string CssClass { get; set; }
        [SitecoreInfo(SitecoreInfoType.Url, UrlOptions = SitecoreInfoUrlOptions.SiteResolving | SitecoreInfoUrlOptions.LanguageEmbeddingAlways)]
        string Url { get; set; }
    }
}
