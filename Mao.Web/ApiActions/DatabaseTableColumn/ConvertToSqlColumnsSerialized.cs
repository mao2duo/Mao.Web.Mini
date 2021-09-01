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
    /// <summary>
    /// 把 DatabaseTableColumn 轉換成 SqlColumn 的序列化結果
    /// </summary>
    public class ConvertToSqlColumnsSerialized
    {
        public class Request : IRequest<Response>
        {
            public ICollection<DatabaseTableColumn> Columns { get; set; }
        }

        public class Response
        {
            public bool IsSuccessed { get; set; }
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
                    request.Columns
                        .OrderBy(x => x.Sort)
                        .Select(x => ObjectResolver.TypeConvert<SqlColumn>(x)),
                    Formatting.Indented);
                response.IsSuccessed = true;
                return response;
            }
        }
    }
}