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
    public class UpdateDatabaseTableList
    {
        public class Request : IRequest<Response>
        {
            public Guid DatabaseId { get; set; }
            public ICollection<DatabaseTable> Tables { get; set; }
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
                if (request.Tables != null)
                {
                    using (var conn = _repository.CreateConnection())
                    {
                        conn.Open();
                        using (var tran = conn.BeginTransaction())
                        {
                            _repository.Delete<DatabaseTableColumn>("DatabaseId", request.DatabaseId, tran);
                            _repository.Delete<DatabaseTable>("DatabaseId", request.DatabaseId, tran);
                            foreach (var databaseTable in request.Tables)
                            {
                                databaseTable.DatabaseId = request.DatabaseId;
                                _repository.Insert(databaseTable, tran);
                                if (databaseTable.Columns != null)
                                {
                                    int sort = 1;
                                    foreach (var databaseTableColumn in databaseTable.Columns.OrderBy(x => x.Sort))
                                    {
                                        databaseTableColumn.DatabaseId = databaseTable.DatabaseId;
                                        databaseTableColumn.TableName = databaseTable.TableName;
                                        databaseTableColumn.Sort = sort;
                                        _repository.Insert(databaseTableColumn, tran);
                                        sort++;
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