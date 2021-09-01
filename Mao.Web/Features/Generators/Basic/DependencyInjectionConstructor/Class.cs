using Mao.Web.ApiActions;
using Mao.Web.Areas.Generate.Views.Generate.Basic.DependencyInjectionConstructor;
using Mao.Web.Features.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;

namespace Mao.Web.Features.Generators.Basic.DependencyInjectionConstructor
{
    public class Class : IGenerator<Input>
    {
        public GenerateOutputFiles.Response.Files Generate(Input input)
        {
            var content = $@"
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace {input.Namespace}
{{
    public class {input.ClassName}
    {{
        {Invoker.UsingIf(Invoker.Using(new Regex(@"^I[A-Z]"),
            regex => input.InjectTypes?
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
        public {input.ClassName}({injectTypes
                                        .Select(x => $"{x.Key} {x.Value}")
                                        .Join(@", ")})
        {{
            {injectTypes
                .Select(x => $"_{x.Value} = {x.Value};")
                .Join(@"
            ")}
        }}".TrimStart())}
    }}
}}".TrimStart('\r', '\n');
            return new GenerateOutputFiles.Response.File()
            {
                Name = $"{input.ClassName}.cs",
                Content = content
            };
        }
    }
}