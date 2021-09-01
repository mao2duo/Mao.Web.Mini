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
    public class DeleteDatabase
    {
        public class Request : IRequest<Response>
        {
            public Guid Id { get; set; }
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
                using (var conn = _repository.CreateConnection())
                {
                    conn.Open();
                    using (var tran = conn.BeginTransaction())
                    {
                        _repository.Delete<DatabaseTableColumn>("DatabaseId", request.Id, tran);
                        _repository.Delete<DatabaseTable>("DatabaseId", request.Id, tran);
                        _repository.Delete<Database.Models.Database>("Id", request.Id, tran);
                        tran.Commit();
                        response.IsSuccessed = true;
                    }
                }
                return response;
            }
        }
    }
}