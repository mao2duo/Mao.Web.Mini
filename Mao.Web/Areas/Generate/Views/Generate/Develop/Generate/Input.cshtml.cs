using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Mao.Web.Areas.Generate.Views.Generate.Develop.Generate
{
    public class Input
    {
        public string Name { get; set; }

        [Required]
        public string Provider { get; set; }

        [Required]
        public string Module { get; set; }
    }
}