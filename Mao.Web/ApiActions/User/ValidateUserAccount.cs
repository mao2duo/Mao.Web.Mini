using Mao.Repository;
using Mao.Web.Database.Models;
using Mao.Web.Features;
using Mao.Web.Features.Interfaces;
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
    /// 將 Account / Password 轉換成 Token
    /// </summary>
    public class ValidateUserAccount
    {
        public class Request : IRequest<Response>
        {
            public string Account { get; set; }
            public string Password { get; set; }
        }

        public class Response
        {
            public bool IsValid { get; set; }
            public Guid UserId { get; set; }
            public string Token { get; set; }
        }

        public class Handler : IRequestHandler<Request, Response>
        {
            private readonly IRepository _repository;
            private readonly IMediator _mediator;
            private readonly IEncryptor _encryptor;
            public Handler(IRepository repository, IMediator mediator, IEnumerable<IEncryptor> encryptors)
            {
                _repository = repository;
                _mediator = mediator;
                _encryptor = encryptors.OfType<Sha512Encryptor>().First();
            }

            public async Task<Response> Handle(Request request, CancellationToken cancellationToken)
            {
                Response response = new Response();
                var passwordHash = (await _mediator.Send(new GetUserPasswordHash.Request()
                {
                    Account = request.Account,
                    Password = request.Password
                })).PasswordHash;
                var user = _repository.SelectTop1<AppUser>(new Dictionary<string, object>()
                {
                    { "Account", request.Account },
                    { "PasswordHash", passwordHash }
                });
                if (user != null)
                {
                    // create token
                    string token = _encryptor.Encrypt($"{user.Id}-{Guid.NewGuid()}");
                    _repository.Delete<AppUserToken>(new Dictionary<string, object>()
                    {
                        { "UserId", user.Id },
                        { "Provider", "Mao.Web" },
                        { "Name", OAuthDefaults.AuthenticationType }
                    });
                    _repository.Insert(new AppUserToken()
                    {
                        UserId = user.Id,
                        Provider = "Mao.Web",
                        Name = OAuthDefaults.AuthenticationType,
                        Value = token
                    });
                    // return
                    response.IsValid = true;
                    response.UserId = user.Id;
                    response.Token = token;
                }
                return response;
            }
        }
    }
}