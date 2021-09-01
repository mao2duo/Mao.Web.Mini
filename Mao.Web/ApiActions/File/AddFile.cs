using Mao.Repository;
using Mao.Web.Database.Models;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Mao.Web.ApiActions
{
    public class AddFile
    {
        public class Request : IRequest<Response>
        {
            public AppFile File { get; set; }
        }

        public class Response
        {
            public Guid Id { get; set; }
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
                if (request.File != null)
                {
                    request.File.Id = Guid.NewGuid();
                    request.File.CreatedAt = DateTime.Now;
                    _repository.Insert(request.File);
                    response.Id = request.File.Id;
                }
                return response;
            }
        }
    }
}