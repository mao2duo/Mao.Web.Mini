using Mao.Repository;
using Mao.Web.Database.Models;
using MediatR;
using SqlKata;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Web;

namespace Mao.Web.ApiActions
{
    public class GetDatabaseList
    {
        public class Request : IRequest<Response>
        {
            public Guid UserId { get; set; }
        }

        public class Response
        {
            public ICollection<Database.Models.Database> List { get; set; }
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
                Query query = new Query(_repository.GetTableName(typeof(Database.Models.Database)))
                    .Where("UserId", request.UserId);
                response.List = _repository.Query<Database.Models.Database>(query).ToList();
                return response;
            }
        }
    }
}