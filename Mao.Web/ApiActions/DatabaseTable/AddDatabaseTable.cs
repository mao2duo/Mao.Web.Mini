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
    public class AddDatabaseTable
    {
        public class Request : IRequest<Response>
        {
            public DatabaseTable Table { get; set; }
        }

        public class Response
        {
            public bool IsExists { get; set; }
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
                if (request.Table != null)
                {
                    var count = _repository.Count<DatabaseTable>(new Dictionary<string, object>()
                    {
                        { "DatabaseId", request.Table.DatabaseId },
                        { "TableName", request.Table.TableName }
                    });
                    if (count > 0)
                    {
                        response.IsExists = true;
                    }
                    else
                    {
                        using (var conn = _repository.CreateConnection())
                        {
                            conn.Open();
                            using (var tran = conn.BeginTransaction())
                            {
                                _repository.Insert(request.Table, tran);
                                if (request.Table.Columns != null)
                                {
                                    foreach (var databaseTableColumn in request.Table.Columns)
                                    {
                                        databaseTableColumn.DatabaseId = request.Table.DatabaseId;
                                        databaseTableColumn.TableName = request.Table.TableName;
                                        _repository.Insert(databaseTableColumn, tran);
                                    }
                                }
                                tran.Commit();
                                response.IsSuccessed = true;
                            }
                        }
                    }
                }
                return response;
            }
        }
    }
}