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
    public class Update : IGenerator<Input>
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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Web;

namespace Mao.Web.ApiActions
{{
    public class Update{input.TableAlias.ToUpperCamelCase()}
    {{
        public class Request : IRequest<Response>
        {{
            public {input.TableName} {input.TableAlias.ToUpperCamelCase()} {{ get; set; }}
            public ICollection<string> UpdateColumnNames {{ get; set; }}
        }}

        public class Response
        {{
            public bool IsSuccessed {{ get; set; }}
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
                if (request.{input.TableAlias.ToUpperCamelCase()} != null)
                {{
                    if (request.UpdateColumnNames != null && request.UpdateColumnNames.Any())
                    {{
                        _repository.Update(request.{input.TableAlias.ToUpperCamelCase()}, request.UpdateColumnNames);
                        response.IsSuccessed = true;
                    }}
                    else
                    {{
                        _repository.Update(request.{input.TableAlias.ToUpperCamelCase()});
                        response.IsSuccessed = true;
                    }}
                }}
                return response;
            }}
        }}
    }}
}}".TrimStart('\r', '\n');
            return new GenerateOutputFiles.Response.File()
            {
                DirectoryPath = $@"{{0}}\{input.TableAlias.ToUpperCamelCase()}",
                Name = $"Update{input.TableAlias.ToUpperCamelCase()}.cs",
                Content = content
            };
        }
    }
}