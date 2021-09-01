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
    public class GetDatabaseTableList
    {
        public class Request : IRequest<Response>
        {
            public Guid DatabaseId { get; set; }
            public bool IncludeColumns { get; set; }
        }

        public class Response
        {
            public ICollection<DatabaseTable> List { get; set; }
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
                Query query = new Query(_repository.GetTableName(typeof(DatabaseTable)))
                    .Where("DatabaseId", request.DatabaseId);
                response.List = _repository.Query<DatabaseTable>(query).ToList();
                if (response.List != null && response.List.Any() && request.IncludeColumns)
                {
                    foreach (var table in response.List)
                    {
                        table.Columns = _repository.Select<DatabaseTableColumn>(new Dictionary<string, object>
                        {
                            { "DatabaseId", request.DatabaseId },
                            { "TableName", table.TableName }
                        }).ToList();
                    }
                }
                return response;
            }
        }
    }
}