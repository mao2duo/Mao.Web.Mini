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
    public class List : IGenerator<Input>
    {
        public GenerateOutputFiles.Response.Files Generate(Input input)
        {
            var columns = string.IsNullOrWhiteSpace(input.TableColumnsJson) ?
                new DatabaseTableColumn[0] :
                JsonConvert.DeserializeObject<DatabaseTableColumn[]>(input.TableColumnsJson);
            var content = $@"
<div class=""row"">
    <div class=""col-md-12 mt-3 mb-3 h2"">
        {input.TableDescription}列表
    </div>
    <div class=""col-md-12 mb-3"">
        <a class=""btn btn-success"" href=""javascript: add{input.TableAlias.ToUpperCamelCase()}();"">新增</a>
    </div>
    <div class=""col-md-12 mb-3"">
        <div class=""table-responsive"">
            <table id=""table-{input.TableAlias.ToLowerSymbolCase("-")}""></table>
        </div>
    </div>
</div>
<script type=""text/javascript"">
    $(function () {{
        $(""#table-{input.TableAlias.ToLowerSymbolCase("-")}"").bootstrapTable($.extend(true, {{}}, UI.component.bootstrapTable.defaultOptions, {{
            ajax: function (request) {{
                API.{input.TableAlias.ToLowerCamelCase()}.list({{
                }}).then(function (response) {{
                    request.success(response.List);
                }});
            }},
            columns: [
                {{
                    width: 50,
                    align: ""center"",
                    formatter: UI.component.bootstrapTable.rowNumberFormatter
                }}, {columns
                        .Where(x => input.TableColumnGenerateSettings
                            .Any(y => y.ColumnName == x.ColumnName
                                && y.OnList?.Features != null
                                && y.OnList.Features.Contains("TableColumn")))
                        .Select(x => $@"
                {{
                    title: ""<span>{x.Description}<span>"",
                    field: ""{x.ColumnName}"",
                }}".TrimStart())
                        .Join(", ")}, {{
                    width: 150,
                    align: ""center"",
                    formatter: buttonsFormatter,
                }},
            ],
        }}));
    }});

    function buttonsFormatter(value, row, index, field) {{
        var buttons = $(""<div></div>"");
        buttons.append(
            $(""<a class=\""btn btn-xs\""><span class=\""fas fa-edit\""></span></a>"")
                .attr(""href"", ""javascript: update{input.TableAlias.ToUpperCamelCase()}("" + {columns
                                                                                                    .Where(x => x.IsPrimaryKey)
                                                                                                    .Select(x => $@"JSON.stringify(row.{x.ColumnName})")
                                                                                                    .Join(@" + "", "" + ")} + "");""));
        buttons.append(
            $(""<a class=\""btn btn-xs\""><span class=\""fas fa-trash-alt\""></span></a>"")
                .attr(""href"", ""javascript: delete{input.TableAlias.ToUpperCamelCase()}("" + {columns
                                                                                                    .Where(x => x.IsPrimaryKey)
                                                                                                    .Select(x => $@"JSON.stringify(row.{x.ColumnName})")
                                                                                                    .Join(@" + "", "" + ")} + "");""));
        return buttons.html();
    }}

    function add{input.TableAlias.ToUpperCamelCase()}() {{
        location.href = ""@Url.Action(""Add"", ""{input.TableAlias.ToUpperCamelCase()}"")"";
    }}
    function update{input.TableAlias.ToUpperCamelCase()}({columns
                                                            .Where(x => x.IsPrimaryKey)
                                                            .Select(x => x.ColumnName.ToLowerCamelCase())
                                                            .Join(", ")}) {{
        location.href = ""@Url.Action(""Update"", ""{input.TableAlias.ToUpperCamelCase()}"")"" + ""?"" + {columns
                                                                                                            .Where(x => x.IsPrimaryKey)
                                                                                                            .Select(x => $@"""{x.ColumnName.ToUpperCamelCase()}="" + encodeURIComponent({x.ColumnName.ToLowerCamelCase()})")
                                                                                                            .Join(@" + ""&"" + ")};
    }}
    function delete{input.TableAlias.ToUpperCamelCase()}({columns
                                                            .Where(x => x.IsPrimaryKey)
                                                            .Select(x => x.ColumnName.ToLowerCamelCase())
                                                            .Join(", ")}) {{
        UI.confirm(""確定要刪除{input.TableDescription}嗎"", ""刪除{input.TableDescription}確認"")
            .then(function (isConfirmed) {{
                if (isConfirmed) {{
                    return API.{input.TableAlias.ToLowerCamelCase()}.delete({{
                        {columns
                            .Where(x => x.IsPrimaryKey)
                            .Select(x => $"{x.ColumnName}: {x.ColumnName.ToLowerCamelCase()}")
                            .Join(@",
                        ")}
                    }});
                }}
            }}).then(function (response) {{
                if (response.IsSuccessed) {{
                    UI.notice(""刪除{input.TableDescription}成功"", ""success"");
                    $(""#table-{input.TableAlias.ToLowerSymbolCase("-")}"").bootstrapTable(""refresh"");
                }}
            }});
    }}
</script>".TrimStart('\r', '\n');
            return new GenerateOutputFiles.Response.File()
            {
                DirectoryPath = $@"{{0}}\{input.TableAlias.ToUpperCamelCase()}",
                Name = $"List.cshtml",
                Content = content
            };
        }
    }
}