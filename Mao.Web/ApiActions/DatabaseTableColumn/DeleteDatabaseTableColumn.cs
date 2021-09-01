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
    public class DeleteDatabaseTableColumn
    {
        public class Request : IRequest<Response>
        {
            public Guid DatabaseId { get; set; }
            public string TableName { get; set; }
            public string ColumnName { get; set; }
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
                _repository.Delete<DatabaseTableColumn>(new Dictionary<string, object>()
                {
                    { "DatabaseId", request.DatabaseId },
                    { "TableName", request.TableName },
                    { "ColumnName", request.ColumnName }
                });
                response.IsSuccessed = true;
                return response;
            }
        }
    }
}