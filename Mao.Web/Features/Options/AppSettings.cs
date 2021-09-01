using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Mao.Web.Features.Options
{
    public class AppSettings
    {
        public string LogPath { get; set; }

        public string AesKey { get; set; }
        public string AesIV { get; set; }
    }
}