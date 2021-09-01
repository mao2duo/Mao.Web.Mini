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
    public class Add : IGenerator<Input>
    {
        public GenerateOutputFiles.Response.Files Generate(Input input)
        {
            var columns = string.IsNullOrWhiteSpace(input.TableColumnsJson) ?
                new DatabaseTableColumn[0] :
                JsonConvert.DeserializeObject<DatabaseTableColumn[]>(input.TableColumnsJson);
            var responseColumn = columns.FirstOrDefault(x => x.IsIdentity) ??
                Invoker.UsingIf(columns.Where(x => x.IsPrimaryKey),
                    primaryKeys => primaryKeys.Count() == 1 && primaryKeys.Any(x => x.TypeFullName == "uniqueidentifier"),
                    primaryKeys => primaryKeys.First());
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
    public class Add{input.TableAlias.ToUpperCamelCase()}
    {{
        public class Request : IRequest<Response>
        {{
            public {input.TableName} {input.TableAlias.ToUpperCamelCase()} {{ get; set; }}
        }}

        public class Response
        {{
            public bool IsExists {{ get; set; }}
            public bool IsSuccessed {{ get; set; }}{Invoker.UsingIf(ObjectResolver.TypeConvert<CsProperty>(ObjectResolver.TypeConvert<SqlColumn>(responseColumn)),
                x => x != null,
                x => $@"
            public {x.TypeName} {x.Name} {{ get; set; }}")}
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
                    // 判斷是否有主索引鍵的重複項目
                    var count = _repository.Count<{input.TableName}>(new Dictionary<string, object>()
                    {{
                        {columns
                            .Where(x => x.IsPrimaryKey && !x.IsIdentity)
                            .Select(x => $@"{{ ""{x.ColumnName}"", request.{input.TableAlias.ToUpperCamelCase()}.{x.ColumnName} }}")
                            .Join(@",
                        ")}
                    }});
                    if (count > 0)
                    {{
                        response.IsExists = true;
                    }}
                    else
                    {{{Invoker.UsingIf(responseColumn,
                            x => x != null && x.TypeFullName == "uniqueidentifier",
                            x => $@"
                        request.{input.TableAlias.ToUpperCamelCase()}.{x.ColumnName} = Guid.NewGuid();")}
                        _repository.Insert(request.{input.TableAlias.ToUpperCamelCase()});
                        response.IsSuccessed = true;{Invoker.UsingIf(responseColumn,
                            x => x != null,
                            x => $@"
                        response.{x.ColumnName} = request.{input.TableAlias.ToUpperCamelCase()}.{x.ColumnName};")}
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
                Name = $"Add{input.TableAlias.ToUpperCamelCase()}.cs",
                Content = content
            };
        }
    }
}