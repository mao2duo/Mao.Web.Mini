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

namespace Mao.Web.Features.Generators.MaoWebMini.TableManage.Controllers
{
    public class Controller : IGenerator<Input>
    {
        public GenerateOutputFiles.Response.Files Generate(Input input)
        {
            var columns = string.IsNullOrWhiteSpace(input.TableColumnsJson) ?
                new DatabaseTableColumn[0] :
                JsonConvert.DeserializeObject<DatabaseTableColumn[]>(input.TableColumnsJson);
            var content = $@"
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Mao.Web.Controllers
{{
    public class {input.TableAlias.ToUpperCamelCase()}Controller : Controller
    {{
        public ActionResult List()
        {{
            return View();
        }}

        {(input.AddAndUpdateView == "Same" ?
            $@"
        public ActionResult Edit()
        {{
            return View();
        }}" :
            $@"
        public ActionResult Add()
        {{
            return View();
        }}

        public ActionResult Update()
        {{
            return View();
        }}").TrimStart()}
    }}
}}
".TrimStart('\r', '\n');
            return new GenerateOutputFiles.Response.File()
            {
                Name = $"{input.TableAlias.ToUpperCamelCase()}Controller.cs",
                Content = content
            };
        }
    }
}