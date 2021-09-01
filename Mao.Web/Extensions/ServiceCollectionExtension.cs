using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http.Controllers;
using System.Web.Mvc;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class ServiceCollectionExtension
    {
        public static IServiceCollection AddControllers(this IServiceCollection services)
        {
            var types = System.Reflection.Assembly.GetExecutingAssembly()
                .GetExportedTypes()
                .Where(x => !x.IsAbstract && !x.IsGenericTypeDefinition && typeof(IController).IsAssignableFrom(x))
                .ToList();
            foreach (var type in types)
            {
                services.AddTransient(type);
            }
            return services;
        }

        public static IServiceCollection AddApiControllers(this IServiceCollection services)
        {
            var types = System.Reflection.Assembly.GetExecutingAssembly()
                .GetExportedTypes()
                .Where(x => !x.IsAbstract && !x.IsGenericTypeDefinition && typeof(IHttpController).IsAssignableFrom(x))
                .ToList();
            foreach (var type in types)
            {
                services.AddTransient(type);
            }
            return services;
        }
    }
}