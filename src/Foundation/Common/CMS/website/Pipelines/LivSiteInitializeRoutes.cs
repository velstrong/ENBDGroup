using Sitecore.Pipelines;
using System.Web.Http;
using System.Web.Routing;

namespace LivApp.Foundation.CMS.Pipelines
{
    public class LivSiteInitializeRoutes
    {
        public virtual void Process(PipelineArgs args)
        {
            RegisterRoute(RouteTable.Routes);
        }
        protected virtual void RegisterRoute(RouteCollection routes)
        {
            RouteTable.Routes.MapHttpRoute(
                 name: "InstagramAPI",
                routeTemplate: "liv/social/instagramfeeds",
                defaults: new { controller= "SocialFeedsAPI", action = "GetInstagramFeeds", id = RouteParameter.Optional }
            );

            RouteTable.Routes.MapHttpRoute(
                 name: "TwitterAPI",
                routeTemplate: "liv/social/twitterfeeds",
                defaults: new { controller = "SocialFeedsAPI", action = "GetTwitterFeeds", id = RouteParameter.Optional }
            );

            RouteTable.Routes.MapHttpRoute(
                name: "FacebookAPI",
               routeTemplate: "liv/social/facebookfeeds",
               defaults: new { controller = "SocialFeedsAPI", action = "GetFacebookFeeds", id = RouteParameter.Optional }
           );
            RouteTable.Routes.MapHttpRoute(
                name: "YoutubeAPI",
               routeTemplate: "liv/social/youtubefeeds",
               defaults: new { controller = "SocialFeedsAPI", action = "GetYoutubeFeeds", id = RouteParameter.Optional }
           );
        }
    }
}
