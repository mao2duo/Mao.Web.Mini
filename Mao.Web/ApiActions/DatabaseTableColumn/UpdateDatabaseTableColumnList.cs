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
    public class UpdateDatabaseTableColumnList
    {
        public class Request : IRequest<Response>
        {
            public Guid DatabaseId { get; set; }
            public string TableName { get; set; }
            public ICollection<DatabaseTableColumn> Columns { get; set; }
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
                if (request.Columns != null)
                {
                    using (var conn = _repository.CreateConnection())
                    {
                        conn.Open();
                        using (var tran = conn.BeginTransaction())
                        {
                            _repository.Delete<DatabaseTableColumn>(new Dictionary<string, object>()
                            {
                                { "DatabaseId", request.DatabaseId },
                                { "TableName", request.TableName }
                            }, tran);
                            int sort = 1;
                            foreach (var column in request.Columns.OrderBy(x => x.Sort))
                            {
                                column.DatabaseId = request.DatabaseId;
                                column.TableName = request.TableName;
                                column.Sort = sort;
                                _repository.Insert(column, tran);
                                sort++;
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