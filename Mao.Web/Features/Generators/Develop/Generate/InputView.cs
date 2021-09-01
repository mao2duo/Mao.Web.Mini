using Mao.Web.ApiActions;
using Mao.Web.Areas.Generate.Views.Generate.Develop.Generate;
using Mao.Web.Features.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Mao.Web.Features.Generators.Develop.Generate
{
    public class InputView : IGenerator<Input>
    {
        public GenerateOutputFiles.Response.Files Generate(Input input)
        {
            var content = $@"
@{{
    Layout = ""~/Areas/Generate/Views/Generate/Input.cshtml"";
}}

<div class=""row"">
    <div class=""col-md-12 form-group"">
        <label></label>
        <input type=""text"" class=""form-control"" name="""" />
    </div>
</div>".TrimStart();
            return new GenerateOutputFiles.Response.File()
            {
                DirectoryPath = $@"Areas\Generate\Views\Generate\{input.Provider}\{input.Module}",
                Name = "Input.cshtml",
                Content = content
            };
        }
    }
}