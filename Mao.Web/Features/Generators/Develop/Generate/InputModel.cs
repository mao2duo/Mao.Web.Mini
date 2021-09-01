using Mao.Web.ApiActions;
using Mao.Web.Areas.Generate.Views.Generate.Develop.Generate;
using Mao.Web.Features.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Mao.Web.Features.Generators.Develop.Generate
{
    public class InputModel : IGenerator<Input>
    {
        public GenerateOutputFiles.Response.Files Generate(Input input)
        {
            var content = $@"
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Mao.Web.Areas.Generate.Views.Generate.{input.Provider}.{input.Module}
{{
    public class Input
    {{
    }}
}}".TrimStart();
            return new GenerateOutputFiles.Response.File()
            {
                DirectoryPath = $@"Areas\Generate\Views\Generate\{input.Provider}\{input.Module}",
                Name = "Input.cshtml.cs",
                Content = content
            };
        }
    }
}