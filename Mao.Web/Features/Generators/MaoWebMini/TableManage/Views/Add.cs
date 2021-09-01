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

namespace Mao.Web.Features.Generators.MaoWebMini.TableManage.Views
{
    public class Add : IGenerator<Input>
    {
        public GenerateOutputFiles.Response.Files Generate(Input input)
        {
            if (input.AddAndUpdateView == "Same")
            {
                return null;
            }
            var columns = string.IsNullOrWhiteSpace(input.TableColumnsJson) ?
                new DatabaseTableColumn[0] :
                JsonConvert.DeserializeObject<DatabaseTableColumn[]>(input.TableColumnsJson);
            var content = $@"
<div class=""row"">
    <div class=""col-md-12 mt-3 mb-3 h2"">
        新增{input.TableDescription}
    </div>
    {columns
        .Where(x => !x.IsIdentity)
        .Select(x => $@"
    <div class=""col-md-12 form-group"">
        <label>{x.Description}</label>
        <input type=""text"" class=""form-control"" id=""{x.ColumnName}"" autocomplete=""off"" />
    </div>")
        .Join()
        .TrimStart()}
    <div class=""col-12"">
        <a class=""btn btn-primary"" href=""javascript: add{input.TableAlias.ToUpperCamelCase()}();"">儲存</a>
        <a class=""btn btn-secondary"" href=""javascript: to{input.TableAlias.ToUpperCamelCase()}List();"">取消</a>
    </div>
</div>
<script type=""text/javascript"">
    $(function () {{

    }});

    function add{input.TableAlias.ToUpperCamelCase()}() {{
        API.{input.TableAlias.ToLowerCamelCase()}.create({{
            {input.TableAlias.ToUpperCamelCase()}: {{
                {columns
                    .Where(x => !x.IsIdentity)
                    .Select(x => $@"{x.ColumnName}: $(""#{x.ColumnName}"").val()")
                    .Join(@",
                ")}
            }}
        }}).then(function (response) {{
            if (response.IsSuccessed) {{
                UI.noticeEnqueue(""新增{input.TableDescription}完成"", ""success"");
                to{input.TableAlias.ToUpperCamelCase()}List();
            }}
        }});
    }}
    function to{input.TableAlias.ToUpperCamelCase()}List() {{
        location.href = ""@Url.Action(""List"", ""{input.TableAlias.ToUpperCamelCase()}"")"";
    }}
</script>".TrimStart('\r', '\n');
            return new GenerateOutputFiles.Response.File()
            {
                DirectoryPath = $@"{{0}}\{input.TableAlias.ToUpperCamelCase()}",
                Name = $"Add.cshtml",
                Content = content
            };
        }
    }
}