using Mao.Repository;
using Mao.Web.Database.Models;
using MediatR;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Web;

namespace Mao.Web.ApiActions
{
    public class GetDatabaseTable
    {
        public class Request : IRequest<Response>
        {
            public Guid DatabaseId { get; set; }
            public string TableName { get; set; }
            public bool IncludeColumns { get; set; }
        }

        public class Response
        {
            public DatabaseTable Table { get; set; }
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
                var databaseTable = _repository.SelectTop1<DatabaseTable>(new Dictionary<string, object>()
                {
                    { "DatabaseId", request.DatabaseId },
                    { "TableName", request.TableName }
                });
                if (request.IncludeColumns)
                {
                    databaseTable.Columns = _repository.Select<DatabaseTableColumn>(new Dictionary<string, object>()
                    {
                        { "DatabaseId", request.DatabaseId },
                        { "TableName", request.TableName }
                    }, "Sort", ListSortDirection.Ascending).ToArray();
                }
                response.Table = databaseTable;
                return response;
            }
        }
    }
}