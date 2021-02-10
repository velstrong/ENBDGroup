using Glass.Mapper.Sc.Configuration;
using Glass.Mapper.Sc.Maps;
using ENBDGroup.Foundation.Common.ORM.Models;

namespace ENBDGroup.Foundation.Common.ORM
{
    public class GlassMappings : SitecoreGlassMap<IGlassBase>
    {
        public override void Configure()
        {
            Map(config =>
            {
                config.AutoMap();
                config.Info(f => f.BaseTemplateIds).InfoType(SitecoreInfoType.BaseTemplateIds);
            });
        }
    }
}