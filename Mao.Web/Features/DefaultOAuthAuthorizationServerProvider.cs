using Mao.Web.ApiActions.User;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.OAuth;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;

namespace Mao.Web.Features
{
    public class DefaultOAuthAuthorizationServerProvider : OAuthAuthorizationServerProvider
    {
        private readonly IServiceProvider _serviceProvider;
        public DefaultOAuthAuthorizationServerProvider(string clientId, IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public override async Task ValidateClientAuthentication(OAuthValidateClientAuthenticationContext context)
        {
            await base.ValidateClientAuthentication(context);
            context.Validated();
        }

        public override async Task GrantResourceOwnerCredentials(OAuthGrantResourceOwnerCredentialsContext context)
        {
            await base.GrantResourceOwnerCredentials(context);
        }
    }
}