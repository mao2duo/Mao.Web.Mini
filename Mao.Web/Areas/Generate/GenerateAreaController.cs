using Mao.Web.ApiActions;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Mao.Web.Areas.Generate
{
    public abstract class GenerateAreaController : Controller
    {
        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            base.OnActionExecuting(filterContext);
            var mediator = Resolver.GetService<IMediator>();
            ViewBag.Menus = mediator.Send(new GetMenu.Request()
            {
                Name = "Generate",
                IncludeChildren = true,
                IncludeRoutes = true
            }).Result.Menu?.Children;
        }
    }
}