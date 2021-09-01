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
    public class GetDatabaseTableColumnList
    {
        public class Request : IRequest<Response>
        {
            public Guid DatabaseId { get; set; }
            public string TableName { get; set; }
        }

        public class Response
        {
            public ICollection<DatabaseTableColumn> List { get; set; }
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
                Query query = new Query(_repository.GetTableName(typeof(DatabaseTableColumn)))
                    .Where("DatabaseId", request.DatabaseId)
                    .Where("TableName", request.TableName)
                    .OrderBy("Sort");
                response.List = _repository.Query<DatabaseTableColumn>(query).ToList();
                return response;
            }
        }
    }
}