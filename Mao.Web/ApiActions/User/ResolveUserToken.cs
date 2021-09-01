using Mao.Repository;
using Mao.Web.Database.Models;
using MediatR;
using Microsoft.Owin.Security.OAuth;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Web;

namespace Mao.Web.ApiActions.User
{
    /// <summary>
    /// 驗證 Token
    /// </summary>
    public class ResolveUserToken
    {
        public class Request : IRequest<Response>
        {
            public string Token { get; set; }
        }

        public class Response
        {
            public bool IsValid { get; set; }
            public Guid UserId { get; set; }
        }

        public class Handler : IRequestHandler<Request, Response>
        {
            private readonly IRepository _repository;
            public Handler(IRepository repository)
            {
                _repository = repository;
            }

            public async Task<Response> Handle(Request request, CancellationToken cancellationToken)
            {
                Response response = new Response();
                var userToken = _repository.SelectTop1<AppUserToken>(new Dictionary<string, object>()
                {
                    { "Provider", "Mao.Web" },
                    { "Name", OAuthDefaults.AuthenticationType },
                    { "Value", request.Token }
                });
                if (userToken != null)
                {
                    response.IsValid = true;
                    response.UserId = userToken.UserId;
                }
                return response;
            }
        }

    }
}