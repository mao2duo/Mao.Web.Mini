﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Mao.Web.Database.Models
{
    public class GenerateInput
    {
        [Key]
        public Guid Id { get; set; }

        public Guid UserId { get; set; }

        public string Provider { get; set; }
        public string Module { get; set; }

        public string Name { get; set; }
        public string Description { get; set; }

        public string Serialization { get; set; }
    }
}