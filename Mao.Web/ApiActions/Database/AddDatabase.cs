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
    public class AddDatabase
    {
        public class Request : IRequest<Response>
        {
            public Database.Models.Database Database { get; set; }
        }

        public class Response
        {
            public Guid Id { get; set; }
            public bool IsSuccessed { get; set; }
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
                if (request.Database != null)
                {
                    request.Database.Id = Guid.NewGuid();
                    _repository.Insert(request.Database);
                    response.IsSuccessed = true;
                    response.Id = request.Database.Id;
                }
                return response;
            }
        }
    }
}