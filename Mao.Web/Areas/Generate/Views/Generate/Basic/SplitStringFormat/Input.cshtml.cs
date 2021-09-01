using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Mao.Web.Areas.Generate.Views.Generate.Basic.SplitStringFormat
{
    public class Input
    {
        public string InputData { get; set; }
        public string InputRowSeparator { get; set; }
        public string InputRowParameterSeparator { get; set; }
        public string OutputFormat { get; set; }
        public string OutputSeparator { get; set; }
    }
}