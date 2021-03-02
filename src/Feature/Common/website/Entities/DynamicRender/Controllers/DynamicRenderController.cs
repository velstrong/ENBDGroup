using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using LivApp.Foundation.Content.Services;
using Sitecore.Mvc.Controllers;
using Sitecore.Data.Items;

namespace ENBDGroup.Feature.Common.Website.Entities.DynamicRender.Controllers
{
    public class DynamicRenderController : SitecoreController
    {
        private readonly IBaseService _baseService;

        public DynamicRenderController(IBaseService baseService)
        {
            _baseService = baseService;
        }

        
    }
}