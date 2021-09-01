using Mao.Web.ApiActions;
using Mao.Web.Features;
using Mao.Web.Features.Attributes;
using Microsoft.Owin.Security.OAuth;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;

namespace Mao.Web
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            // 讓 ApiController 不使用 DefaultAuthenticationTypes.ApplicationCookie 的登入資訊
            config.SuppressDefaultHostAuthentication();
            // 讓 ApiController 使用 OAuthDefaults.AuthenticationType 的登入資訊
            config.Filters.Add(new HostAuthenticationFilter(OAuthDefaults.AuthenticationType));
            // 預設驗證 ModelState.IsValid
            config.Filters.Add(new ValidateModelAttribute());

            config.Services.Insert(
                typeof(System.Web.Http.ModelBinding.ModelBinderProvider),
                0,
                new System.Web.Http.ModelBinding.Binders.SimpleModelBinderProvider(
                    typeof(GenerateOutputFiles.Request),
                    new GenerateOutputFilesRequestModelBinder()));

            config.MapHttpAttributeRoutes();

            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );
        }
    }
}
