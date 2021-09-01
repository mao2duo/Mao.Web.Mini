using Mao.Web.ApiActions;
using Mao.Web.Features.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;

namespace Mao.Web.Features.Generators.Basic.MediatR
{
    public class Action : IGenerator<Action.Request>
    {
        public class Request
        {
            public string Namespace { get; set; }
            public string ClassName { get; set; }
            public string[] InjectTypes { get; set; }
        }

        public GenerateOutputFiles.Response.Files Generate(Request request)
        {
            var content = $@"
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace {request.Namespace}
{{
    public class {request.ClassName}
    {{
        public class Request : IRequest<Response>
        {{
        }}

        public class Response
        {{
        }}

        public class Handler : IRequestHandler<Request, Response>
        {{
            {Invoker.UsingIf(Invoker.Using(new Regex(@"^I[A-Z]"),
                regex => request.InjectTypes?
                    .Where(x => !string.IsNullOrWhiteSpace(x))
                    .ToDictionary(
                        x => x.Trim(),
                        x => regex.IsMatch(x.Trim()) ?
                            regex.Replace(x.Trim(), match => match.Value.Substring(1)).ToLowerCamelCase() :
                            x.Trim().ToLowerCamelCase())),
                injectTypes => injectTypes != null,
                injectTypes => $@"
            {injectTypes
                .Select(x => $"private readonly {x.Key} _{x.Value};")
                .Join(@"
            ")}
            public Handler({injectTypes
                                .Select(x => $"{x.Key} {x.Value}")
                                .Join(@", ")})
            {{
                {injectTypes
                    .Select(x => $"_{x.Value} = {x.Value};")
                    .Join(@"
                ")}
            }}
            ".TrimStart())}
            public async Task<Response> Handle(Request request, CancellationToken cancellationToken)
            {{
                Response response = new Response();
                // TODO:
                return response;
            }}
        }}
    }}
}}".TrimStart('\r', '\n');
            return new GenerateOutputFiles.Response.File()
            {
                Name = $"{request.ClassName}.cs",
                Content = content
            };
        }
    }
}