using Mao.Web.ApiActions;
using Mao.Web.Areas.Generate.Views.Generate.MaoWebMini.SearchList;
using Mao.Web.Features.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Mao.Web.Features.Generators.MaoWebMini.SearchList.Controllers
{
    public class Controller : IGenerator<Input>
    {
        public GenerateOutputFiles.Response.Files Generate(Input input)
        {
            var content = $@"";
            return new GenerateOutputFiles.Response.File()
            {
                Name = $"",
                Content = content
            };
        }
    }
}