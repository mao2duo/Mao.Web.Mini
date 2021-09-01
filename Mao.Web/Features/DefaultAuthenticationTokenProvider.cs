using Mao.Web.ApiActions.User;
using Mao.Web.Features.Interfaces;
using MediatR;
using Microsoft.AspNet.Identity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.Infrastructure;
using Microsoft.Owin.Security.OAuth;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;

namespace Mao.Web.Features
{
    public class DefaultAuthenticationTokenProvider : AuthenticationTokenProvider
    {
        private readonly IServiceProvider _serviceProvider;
        public DefaultAuthenticationTokenProvider(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public override async Task CreateAsync(AuthenticationTokenCreateContext context)
        {
            await base.CreateAsync(context);
            //context.SetToken(context.SerializeTicket());
        }

        public override async Task ReceiveAsync(AuthenticationTokenReceiveContext context)
        {
            await base.ReceiveAsync(context);
            //context.DeserializeTicket(context.Token);
            var mediator = _serviceProvider.GetService<IMediator>();
            var resolveUserToken = await mediator.Send(new ResolveUserToken.Request()
            {
                Token = context.Token
            });
            if (resolveUserToken.IsValid)
            {
                var getUserClaims = await mediator.Send(new GetUserClaims.Request()
                {
                    UserId = resolveUserToken.UserId
                });
                var claimsIdentity = new ClaimsIdentity(getUserClaims.Claims, OAuthDefaults.AuthenticationType);
                context.SetTicket(new AuthenticationTicket(claimsIdentity, new AuthenticationProperties(new Dictionary<string, string>
                {
                    { "UserId", claimsIdentity.GetClaimValue(ClaimTypes.Sid) },
                    { "Account", claimsIdentity.GetClaimValue(ClaimTypes.NameIdentifier) },
                    { "Name", claimsIdentity.GetClaimValue(ClaimTypes.Name) }
                })));
            }
        }
    }
}