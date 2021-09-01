using Mao.Repository;
using Mao.Web.Database.Models;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using System.Web;

namespace Mao.Web.ApiActions.User
{
    /// <summary>
    /// 以 UserId 取得 Claims
    /// </summary>
    public class GetUserClaims
    {
        public class Request : IRequest<Response>
        {
            public Guid UserId { get; set; }
        }

        public class Response
        {
            public bool IsValid { get; set; }
            public IEnumerable<Claim> Claims { get; set; }
        }

        public class Handler : IRequestHandler<Request, Response>
        {
            private readonly IRepository _repository;
            private readonly IMediator _mediator;
            public Handler(IRepository repository, IMediator mediator)
            {
                _repository = repository;
                _mediator = mediator;
            }

            public async Task<Response> Handle(Request request, CancellationToken cancellationToken)
            {
                Response response = new Response();
                AppUser user = _repository.SelectTop1<AppUser>("Id", request.UserId);
                if (user != null)
                {
                    // 在這裡寫要存到 cookie 的使用者資訊
                    List<Claim> claims = new List<Claim>();
                    claims.Add(new Claim(ClaimTypes.Sid, user.Id.ToString()));
                    claims.Add(new Claim(ClaimTypes.NameIdentifier, user.Account));
                    claims.Add(new Claim(ClaimTypes.Name, user.DisplayName));
                    //claims.Add(new Claim($"{ClaimTypes.UserData}/language", request.UserLanguage ?? ""));
                    response.IsValid = true;
                    response.Claims = claims;
                }
                return response;
            }
        }
    }
}