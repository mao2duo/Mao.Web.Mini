using Mao.Web.ApiActions;
using Mao.Web.Areas.Generate.Views.Generate.Develop.Generate;
using Mao.Web.Features.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Mao.Web.Features.Generators.Develop.Generate
{
    public class Generator : IGenerator<Input>
    {
        public GenerateOutputFiles.Response.Files Generate(Input input)
        {
            if (string.IsNullOrWhiteSpace(input.Name))
            {
                input.Name = "Output";
            }
            var content = $@"
using Mao.Web.ApiActions;
using Mao.Web.Areas.Generate.Views.Generate.{input.Provider}.{input.Module};
using Mao.Web.Features.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Mao.Web.Features.Generators.{input.Provider}.{input.Module}
{{
    public class {input.Name} : IGenerator<Input>
    {{
        public GenerateOutputFiles.Response.Files Generate(Input input)
        {{
            var content = $@"""";
            return new GenerateOutputFiles.Response.File()
            {{
                Name = $""{input.Name}"",
                Content = content
            }};
        }}
    }}
}}".TrimStart('\r', '\n');
            return new GenerateOutputFiles.Response.File()
            {
                DirectoryPath = $@"Features\Generators\{input.Provider}\{input.Module}",
                Name = $"{input.Name}.cs",
                Content = content
            };
        }
    }
}