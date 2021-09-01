using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Mao.Web.Areas.Generate.Views.Generate.Basic.DependencyInjectionConstructor
{
    public class Input
    {
        public string Namespace { get; set; }
        public string ClassName { get; set; }
        public string[] InjectTypes { get; set; }
    }
}