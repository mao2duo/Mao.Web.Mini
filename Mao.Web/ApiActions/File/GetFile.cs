using Mao.Repository;
using Mao.Web.Database.Models;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Web;

namespace Mao.Web.ApiActions
{
    public class GetFile
    {
        public class Request : IRequest<Response>
        {
            public Guid Id { get; set; }
        }

        public class Response
        {
            public string Name { get; set; }
            /// <summary>
            /// Base64
            /// </summary>
            public string Content { get; set; }
            public string ContentType { get; set; }
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
                AppFile file = _repository.SelectTop1<AppFile>("Id", request.Id);
                if (file != null)
                {
                    response.Name = file.Name;
                    response.Content = file.Content;
                    response.ContentType = file.ContentType;
                    if (file.IsDisposable)
                    {
                        _repository.Delete<AppFile>("Id", request.Id);
                    }
                }
                return response;
            }
        }
    }
}