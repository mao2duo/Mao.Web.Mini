using Mao.Generate.Models;
using Mao.Repository;
using Mao.Web.Database.Models;
using MediatR;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Web;

namespace Mao.Web.ApiActions
{
    public class ConvertToSqlTablesSerialized
    {
        public class Request : IRequest<Response>
        {
            public ICollection<DatabaseTable> Tables { get; set; }
        }

        public class Response
        {
            public bool IsSuccessed { get; set; }
            public string Message { get; set; }
            public string Serialized { get; set; }
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
                response.Serialized = JsonConvert.SerializeObject(
                    request.Tables
                        .Select(x => ObjectResolver.TypeConvert<SqlTable>(x)),
                    Formatting.Indented);
                response.IsSuccessed = true;
                return response;
            }
        }
    }
}