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
    public class UpdateDatabaseTable
    {
        public class Request : IRequest<Response>
        {
            public DatabaseTable Table { get; set; }
            public ICollection<string> UpdateColumnNames { get; set; }
            public bool IncludeColumns { get; set; }
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
                if (request.Table != null)
                {
                    using (var conn = _repository.CreateConnection())
                    {
                        conn.Open();
                        using (var tran = conn.BeginTransaction())
                        {
                            if (request.UpdateColumnNames != null && request.UpdateColumnNames.Any())
                            {
                                _repository.Update(request.Table, request.UpdateColumnNames, tran);
                            }
                            _repository.Update(request.Table, tran);
                            if (request.IncludeColumns)
                            {
                                _repository.Delete<DatabaseTableColumn>(new Dictionary<string, object>
                                {
                                    { "DatabaseId", request.Table.DatabaseId },
                                    { "TableName", request.Table.TableName }
                                }, tran);
                                if (request.Table.Columns != null)
                                {
                                    foreach (var databaseTableColumn in request.Table.Columns)
                                    {
                                        databaseTableColumn.DatabaseId = request.Table.DatabaseId;
                                        databaseTableColumn.TableName = request.Table.TableName;
                                        _repository.Insert(databaseTableColumn, tran);
                                    }
                                }
                            }
                            tran.Commit();
                            response.IsSuccessed = true;
                        }
                    }
                }
                return response;
            }
        }
    }
}