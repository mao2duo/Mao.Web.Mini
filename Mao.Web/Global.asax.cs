using Mao.Web.Features;
using Mao.Web.Features.Options;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Web;
using System.Web.Hosting;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace Mao.Web
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            GlobalConfiguration.Configure(WebApiConfig.Register);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
        }

        protected void Application_Error(object sender, EventArgs e)
        {
            var serverError = Server.GetLastError();
            var serviceProvider = DependencyResolver.Current.GetService<IServiceProvider>();
            var appSettings = serviceProvider.GetService<AppSettings>();
            string logPath = HostingEnvironment.MapPath(appSettings.LogPath);
            System.IO.Directory.CreateDirectory(logPath);
            string logFilePath = System.IO.Path.Combine(logPath, $"{DateTime.Today:yyyy-MM-dd}.txt");
            System.IO.File.AppendAllText(logFilePath, $@"
{DateTime.Now:HH:mm:ss}
{serverError.Message}
{serverError}
");
            var logger = serviceProvider.GetService<ILogger<MvcApplication>>();
            logger.LogError(serverError, $"Global Exception: {serverError.Message}");
        }
    }
}
