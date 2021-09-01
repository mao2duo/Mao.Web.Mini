using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http.Dependencies;

namespace Mao.Web.Features
{
    public class DefaultDependencyResolver : System.Web.Mvc.IDependencyResolver, IDependencyResolver
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly IServiceScope _serviceScope;
        public DefaultDependencyResolver(IServiceProvider serviceProvider)
        {
            this._serviceProvider = serviceProvider;
        }
        public DefaultDependencyResolver(IServiceScope serviceScope)
        {
            this._serviceScope = serviceScope;
            this._serviceProvider = serviceScope.ServiceProvider;
        }

        public object GetService(Type serviceType)
        {
            return _serviceProvider.GetService(serviceType);
        }
        public IEnumerable<object> GetServices(Type serviceType)
        {
            return _serviceProvider.GetServices(serviceType);
        }

        public IDependencyScope BeginScope()
        {
            return new DefaultDependencyResolver(_serviceProvider.CreateScope());
        }
        public void Dispose()
        {
            _serviceScope?.Dispose();
        }
    }
}