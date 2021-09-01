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
    public class GetMenuList
    {
        public class Request : IRequest<Response>
        {
            public Guid? ParentId { get; set; }
        }

        public class Response
        {
            public ICollection<AppMenu> List { get; set; }
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
                query = query.Where("ParentId", request.ParentId);
                query = query.OrderBy("Sort");
                response.List = _repository.Query<AppMenu>(query).ToList();
                return response;
            }
        }
    }
}