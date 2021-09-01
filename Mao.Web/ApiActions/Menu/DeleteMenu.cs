using Mao.Repository;
using Mao.Web.Database.Models;
using MediatR;
using SqlKata;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Web;

namespace Mao.Web.ApiActions
{
    public class DeleteMenu
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
                        DeleteMenuAndChildren(request.Id, tran);
                        tran.Commit();
                        response.IsSuccessed = true;
                    }
                }
                return response;
            }

            /// <summary>
            /// 移除選單及子選單
            /// </summary>
            private void DeleteMenuAndChildren(Guid id, IDbTransaction tran)
            {
                Query query = new Query(_repository.GetTableName(typeof(AppMenu)));
                query = query.Where("ParentId", id);
                query = query.Select("Id");
                var childIds = _repository.Query<Guid>(query, tran).ToArray();
                if (childIds != null && childIds.Any())
                {
                    foreach (var childId in childIds)
                    {
                        DeleteMenuAndChildren(childId, tran);
                    }
                }
                _repository.Delete<AppMenuRoute>("MenuId", id, tran);
                _repository.Delete<AppMenu>("Id", id, tran);
            }
        }
    }
}