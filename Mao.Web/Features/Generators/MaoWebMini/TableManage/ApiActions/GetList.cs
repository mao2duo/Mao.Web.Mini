using Mao.Generate;
using Mao.Generate.Models;
using Mao.Web.ApiActions;
using Mao.Web.Areas.Generate.Views.Generate.MaoWebMini.TableManage;
using Mao.Web.Database.Models;
using Mao.Web.Features.Interfaces;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Mao.Web.Features.Generators.MaoWebMini.TableManage.ApiActions
{
    public class GetList : IGenerator<Input>
    {
        public GenerateOutputFiles.Response.Files Generate(Input input)
        {
            var columns = string.IsNullOrWhiteSpace(input.TableColumnsJson) ?
                new DatabaseTableColumn[0] :
                JsonConvert.DeserializeObject<DatabaseTableColumn[]>(input.TableColumnsJson);
            var content = $@"
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
{{
    public class Get{input.TableAlias.ToUpperCamelCase()}List
    {{
        public class Request : IRequest<Response>
        {{
        }}

        public class Response
        {{
            public ICollection<{input.TableName}> List {{ get; set; }}
        }}

        public class Handler : IRequestHandler<Request, Response>
        {{
            private readonly IRepository _repository;
            public Handler(IRepository repository)
            {{
                _repository = repository;
            }}

            public async Task<Response> Handle(Request request, CancellationToken cancellationToken)
            {{
                Response response = new Response();
                Query query = new Query(_repository.GetTableName(typeof({input.TableName})));
                response.List = _repository.Query<{input.TableName}>(query).ToList();
                return response;
            }}
        }}
    }}
}}".TrimStart('\r', '\n');
            return new GenerateOutputFiles.Response.File()
            {
                DirectoryPath = $@"{{0}}\{input.TableAlias.ToUpperCamelCase()}",
                Name = $"Get{input.TableAlias.ToUpperCamelCase()}List.cs",
                Content = content
            };
        }
    }
}