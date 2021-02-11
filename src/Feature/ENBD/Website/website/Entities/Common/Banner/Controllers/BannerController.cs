using ENBDGroup.Feature.Enbd.Website.Banner.Models;
using LivApp.Foundation.Content.Services;
using Sitecore.Mvc.Controllers;
using System.Web.Mvc;

namespace ENBDGroup.Feature.Enbd.Website.Banner.Controller
{

    public class BannerController : SitecoreController
    {
        private readonly IBaseService _baseService;

        public BannerController(IBaseService baseService)
        {
            _baseService = baseService;
        }

        public ActionResult VideoBanner()
        {
            var bannerItemDataSource = _baseService.GetDataSourceItem<IVideoBanner>();
            if (bannerItemDataSource != null)
            {
                bannerItemDataSource.CssClass = _baseService.GetRenderingParameter("CSS Class");
                return View("~/Views/Liv/Common/Banner/VideoBanner.cshtml", bannerItemDataSource);
            }
            else
            {
                return View("~/Views/Liv/Common/Error/Error.cshtml");
            }
        }
        public ActionResult YoutubeVideoBanner()
        {
            var bannerItemDataSource = _baseService.GetDataSourceItem<IYoutubeVideoBanner>();
            if (bannerItemDataSource != null)
            {
                bannerItemDataSource.CssClass = _baseService.GetRenderingParameter("CSS Class");
                return View("~/Views/Liv/Common/Banner/YoutubeVideoBanner.cshtml", bannerItemDataSource);
            }
            else
            {
                return View("~/Views/Liv/Common/Error/Error.cshtml");
            }
        }
        public ActionResult HeroBanner()
        {
            var bannerItemDataSource = _baseService.GetDataSourceOrCurrentItem<IHeroBanner>();
            if (bannerItemDataSource != null)
            {
                bannerItemDataSource.CssClass = _baseService.GetRenderingParameter("CSS Class");
                bannerItemDataSource.ContentId = _baseService.GetRenderingParameter("Content Id");
                return View("~/Views/Liv/Common/Banner/HeroBanner.cshtml", bannerItemDataSource);
            }
            else
            {
                return View("~/Views/Liv/Common/Error/Error.cshtml");
            }
        }

       
    }
}