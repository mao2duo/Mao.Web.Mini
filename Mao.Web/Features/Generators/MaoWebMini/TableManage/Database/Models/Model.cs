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

namespace Mao.Web.Features.Generators.MaoWebMini.TableManage.Database.Models
{
    public class Model : IGenerator<Input>
    {
        private readonly CsService _csService;
        public Model(CsService csService)
        {
            _csService = csService;
        }

        public GenerateOutputFiles.Response.Files Generate(Input input)
        {
            var sqlTable = new SqlTable();
            sqlTable.Name = input.TableName;
            sqlTable.Columns = string.IsNullOrWhiteSpace(input.TableColumnsJson) ?
                new SqlColumn[0] :
                JsonConvert.DeserializeObject<DatabaseTableColumn[]>(input.TableColumnsJson).Select(x => ObjectResolver.TypeConvert<SqlColumn>(x)).ToArray();
            var csType = ObjectResolver.TypeConvert<CsType>(sqlTable);
            var content = $@"
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace {input.ProjectName}.Database.Models
{{
{_csService.Stringify(csType).Indent()}
}}".TrimStart('\r', '\n');
            return new GenerateOutputFiles.Response.File()
            {
                Name = $"{input.TableName}.cs",
                Content = content
            };
        }
    }
}