using Mao.Web.ApiActions;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Mao.Web.Areas.Manage
{
    public abstract class ManageAreaController : Controller
    {
        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            base.OnActionExecuting(filterContext);
            var mediator = Resolver.GetService<IMediator>();
            ViewBag.Menus = mediator.Send(new GetMenu.Request()
            {
                Name = "Manage",
                IncludeChildren = true,
                IncludeRoutes = true
            }).Result.Menu?.Children;
        }
    }
}