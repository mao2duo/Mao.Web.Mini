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
    public class AddMenu
    {
        public class Request : IRequest<Response>
        {
            public AppMenu Menu { get; set; }
        }

        public class Response
        {
            public Guid Id { get; set; }
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
                if (request.Menu != null)
                {
                    request.Menu.Id = Guid.NewGuid();
                    using (var conn = _repository.CreateConnection())
                    {
                        conn.Open();
                        using (var tran = conn.BeginTransaction())
                        {
                            if (request.Menu.Routes != null && request.Menu.Routes.Any())
                            {
                                foreach (var menuRoute in request.Menu.Routes)
                                {
                                    menuRoute.MenuId = request.Menu.Id;
                                    _repository.Insert(menuRoute, tran);
                                }
                            }
                            _repository.Insert(request.Menu, tran);
                            tran.Commit();
                            response.IsSuccessed = true;
                        }
                    }
                    response.Id = request.Menu.Id;
                }
                return response;
            }
        }
    }
}