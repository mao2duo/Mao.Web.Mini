using Mao.Web.Features;
using Mao.Web.Features.Interfaces;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Web;

namespace Mao.Web.ApiActions.User
{
    /// <summary>
    /// 將 Password 轉換成 PasswordHash
    /// </summary>
    public class GetUserPasswordHash
    {
        public class Request : IRequest<Response>
        {
            public string Account { get; set; }
            public string Password { get; set; }
        }

        public class Response
        {
            public string PasswordHash { get; set; }
        }

        public class Handler : IRequestHandler<Request, Response>
        {
            private readonly IEncryptor _encryptor;
            public Handler(IEnumerable<IEncryptor> encryptors)
            {
                _encryptor = encryptors.OfType<Sha512Encryptor>().First();
            }

            public async Task<Response> Handle(Request request, CancellationToken cancellationToken)
            {
                Response response = new Response();
                response.PasswordHash = _encryptor.Encrypt(request.Password);
                return response;
            }
        }
    }
}