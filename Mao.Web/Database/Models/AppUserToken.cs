using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Mao.Web.Database.Models
{
    public class AppUserToken
    {
        [Key]
        public Guid UserId { get; set; }

        [Key]
        public string Provider { get; set; }

        [Key]
        public string Name { get; set; }

        public string Value { get; set; }
    }
}