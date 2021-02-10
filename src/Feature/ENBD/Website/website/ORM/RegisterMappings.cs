using Glass.Mapper.Sc.Pipelines.AddMaps;
using ENBDGroup.Foundation.Common.ORM.Extensions;

namespace ENBDGroup.Feature.Liv.Website.ORM
{
    public class RegisterMappings : AddMapsPipeline  {
        public void Process(AddMapsPipelineArgs args)
        {
            args.MapsConfigFactory.AddFluentMaps("ENBDGroup.Feature.Liv.Website");
        }
    }
}