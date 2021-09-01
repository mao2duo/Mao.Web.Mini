using Mao.Web.Database.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace Mao.Web.Extensions
{
    public static class UrlHelperExtension
    {
        /// <summary>
        /// 取得 AppMenu 的連結
        /// </summary>
        public static string MenuUrl(this UrlHelper helper, AppMenu menu)
        {
            if (menu.IsLink)
            {
                if (!string.IsNullOrEmpty(menu.Href))
                {
                    return menu.Href;
                }
                if (menu.Routes != null && menu.Routes.Any())
                {
                    var dictionary = menu.Routes.ToDictionary<AppMenuRoute, string, object>(x => x.Name, x => x.Value);
                    if (!dictionary.TryAdd("menuId", menu.Id))
                    {
                        dictionary["menuId"] = menu.Id;
                    }
                    var routeValues = new RouteValueDictionary(dictionary);
                    return helper.RouteUrl(routeValues);
                }
                return helper.Content("~/");
            }
            return null;
        }
    }
}