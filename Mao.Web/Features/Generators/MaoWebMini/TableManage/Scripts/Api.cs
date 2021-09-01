using Mao.Generate;
using Mao.Web.ApiActions;
using Mao.Web.Areas.Generate.Views.Generate.MaoWebMini.TableManage;
using Mao.Web.Features.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Mao.Web.Features.Generators.MaoWebMini.TableManage.Scripts
{
    public class Api : IGenerator<Input>
    {
        public GenerateOutputFiles.Response.Files Generate(Input input)
        {
            var content = $@"
        {input.TableAlias.ToLowerCamelCase()}: {{
            list: function (data) {{ return get(""/api/{input.TableAlias}/List"", data); }},
            get: function (data) {{ return get(""/api/{input.TableAlias}"", data); }},
            create: function (data) {{ return post(""/api/{input.TableAlias}"", data); }},
            update: function (data) {{ return put(""/api/{input.TableAlias}"", data); }},
            delete: function (data) {{ return del(""/api/{input.TableAlias}"", data); }}
        }},".TrimStart('\r', '\n');
            return new GenerateOutputFiles.Response.File()
            {
                DirectoryPath = "lib",
                Name = $"site.secondary-{input.TableAlias.ToLowerSymbolCase("-")}.js",
                Content = content
            };
        }
    }
}