using Glass.Mapper.Sc.Pipelines.AddMaps;
using ENBDGroup.Foundation.Common.ORM.Extensions;

namespace ENBDGroup.Foundation.Common.ORM.Mappings
{
    public class RegisterMappings : AddMapsPipeline  {
        public void Process(AddMapsPipelineArgs args)
        {
            args.MapsConfigFactory.AddFluentMaps("ENBDGroup.Foundation.Common.ORM");
        }
    }
}