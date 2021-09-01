using Mao.Generate;
using Mao.Repository;
using Mao.Web.Features;
using Mao.Web.Features.Interfaces;
using Mao.Web.Features.Options;
using MediatR;
using Microsoft.AspNet.Identity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Owin;
using Microsoft.Owin.Security.Cookies;
using Microsoft.Owin.Security.OAuth;
using Owin;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Http;
using System.Web.Http.ExceptionHandling;
using System.Web.Mvc;

[assembly: OwinStartup(typeof(Mao.Web.Startup))]
namespace Mao.Web
{
    public class Startup
    {
        public void ConfigureAuth(IAppBuilder app, IServiceProvider serviceProvider)
        {
            // ApplicationCookie for app
            app.UseCookieAuthentication(new CookieAuthenticationOptions
            {
                AuthenticationType = DefaultAuthenticationTypes.ApplicationCookie,
                LoginPath = new PathString("/User/Login"),
                LogoutPath = new PathString("/User/Logout"),
                ExpireTimeSpan = TimeSpan.FromDays(30),
            });

            // Enable the application to use bearer tokens to authenticate users
            var tokenProvider = new DefaultAuthenticationTokenProvider(serviceProvider);
            app.UseOAuthBearerTokens(new OAuthAuthorizationServerOptions
            {
                AccessTokenProvider = tokenProvider
            });
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();
            services.AddApiControllers();

            services.AddScoped(x =>
            {
                var connectionStrings = new ConnectionStrings();
                if (ConfigurationManager.ConnectionStrings.Count > 0)
                {
                    connectionStrings.Default = ConfigurationManager.ConnectionStrings["Default"].ConnectionString;
                }
                return connectionStrings;
            });
            services.AddScoped(x =>
            {
                var appSettings = new AppSettings();
                appSettings.LogPath = ConfigurationManager.AppSettings.Get("LogPath");
                appSettings.AesKey = ConfigurationManager.AppSettings.Get("AesKey");
                appSettings.AesIV = ConfigurationManager.AppSettings.Get("AesIV");
                return appSettings;
            });

            services.AddLogging(logging =>
            {
                //logging.AddConsole();
            });
            services.AddSingleton<IExceptionLogger, DefaultExceptionLogger>();
            services.AddHttpClient();
            services.AddMediatR(Assembly.GetExecutingAssembly());

            services.AddScoped<IEncryptor, Sha256Encryptor>();
            services.AddScoped<IEncryptor, Sha512Encryptor>();
            // 做為預設的 IEncryptor 必須是最後才加入的 IEncryptor
            services.AddScoped<IEncryptor, AesEncryptor>();
            
            services.AddScoped<SqlService>();
            services.AddScoped<CsService>();

            services.AddScoped<IRepository, DefaultRepository>();
        }

        public void Configuration(IAppBuilder app)
        {
            var services = new ServiceCollection();
            ConfigureServices(services);
            var serviceProvider = services.BuildServiceProvider();
            var resolver = new DefaultDependencyResolver(serviceProvider);
            DependencyResolver.SetResolver(resolver);
            GlobalConfiguration.Configuration.DependencyResolver = resolver;
            ConfigureAuth(app, serviceProvider);
        }
    }
}