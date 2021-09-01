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
    public class UpdateDatabaseTableColumn
    {
        public class Request : IRequest<Response>
        {
            public DatabaseTableColumn Column { get; set; }
            public ICollection<string> UpdateColumnNames { get; set; }
        }

        public class Response
        {
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
                if (request.Column != null)
                {
                    if (request.UpdateColumnNames != null && request.UpdateColumnNames.Any())
                    {
                        _repository.Update(request.Column, request.UpdateColumnNames);
                        response.IsSuccessed = true;
                    }
                    else
                    {
                        _repository.Update(request.Column);
                        response.IsSuccessed = true;
                    }
                }
                return response;
            }
        }
    }
}