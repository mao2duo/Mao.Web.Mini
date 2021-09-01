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
    public class GetGenerateInputList
    {
        public class Request : IRequest<Response>
        {
            public Guid UserId { get; set; }
            public string Provider { get; set; }
            public string Module { get; set; }
        }

        public class Response
        {
            public ICollection<GenerateInput> List { get; set; }
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
                Query query = new Query(_repository.GetTableName(typeof(GenerateInput)))
                    .Where("UserId", request.UserId);
                if (!string.IsNullOrEmpty(request.Provider))
                {
                    query = query.Where("Provider", request.Provider);
                }
                if (!string.IsNullOrEmpty(request.Module))
                {
                    query = query.Where("Module", request.Module);
                }
                response.List = _repository.Query<GenerateInput>(query).ToList();
                return response;
            }
        }
    }
}