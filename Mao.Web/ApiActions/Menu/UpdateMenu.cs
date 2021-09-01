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
    public class UpdateMenu
    {
        public class Request : IRequest<Response>
        {
            public AppMenu Menu { get; set; }
            public ICollection<string> UpdateColumnNames { get; set; }
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
                if (request.Menu != null)
                {
                    using (var conn = _repository.CreateConnection())
                    {
                        conn.Open();
                        using (var tran = conn.BeginTransaction())
                        {
                            if (request.Menu.Routes != null)
                            {
                                _repository.Delete<AppMenuRoute>("MenuId", request.Menu.Id, tran);
                                foreach (var route in request.Menu.Routes)
                                {
                                    route.MenuId = request.Menu.Id;
                                    _repository.Insert(route, tran);
                                }
                            }
                            if (request.UpdateColumnNames != null && request.UpdateColumnNames.Any())
                            {
                                _repository.Update(request.Menu, request.UpdateColumnNames);
                            }
                            else
                            {
                                _repository.Update(request.Menu);
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