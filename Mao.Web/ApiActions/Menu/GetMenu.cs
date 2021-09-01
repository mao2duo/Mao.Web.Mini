using Mao.Repository;
using Mao.Web.Database.Models;
using MediatR;
using SqlKata;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Web;

namespace Mao.Web.ApiActions
{
    public class GetMenu
    {
        public class Request : IRequest<Response>
        {
            public Guid? Id { get; set; }
            public string Name { get; set; }
            public bool IncludeChildren { get; set; }
            public bool IncludeRoutes { get; set; }
        }

        public class Response
        {
            public AppMenu Menu { get; set; }
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
                Query query = new Query(_repository.GetTableName(typeof(AppMenu)));
                if (request.Id.HasValue)
                {
                    query = query.Where("Id", request.Id.Value);
                }
                if (!string.IsNullOrEmpty(request.Name))
                {
                    query = query.Where("Name", request.Name);
                }
                query = query.OrderBy("Sort");
                response.Menu = _repository.QueryFirstOrDefault<AppMenu>(query);
                if (response.Menu != null)
                {
                    IncludeRoutesAndChildren(response.Menu, request.IncludeRoutes, request.IncludeChildren);
                }
                return response;
            }

            private void IncludeRoutesAndChildren(AppMenu menu, bool includeRoutes, bool includeChildren)
            {
                // 路由參數
                if (includeRoutes)
                {
                    menu.Routes = _repository.Select<AppMenuRoute>("MenuId", menu.Id).ToArray();
                }
                // 子選單
                if (includeChildren)
                {
                    menu.Children = _repository
                        .Select<AppMenu>("ParentId", menu.Id, "Sort", ListSortDirection.Ascending)
                        .ToArray();
                    foreach (var child in menu.Children)
                    {
                        IncludeRoutesAndChildren(child, includeRoutes, includeChildren);
                    }
                }
            }
        }
    }
}