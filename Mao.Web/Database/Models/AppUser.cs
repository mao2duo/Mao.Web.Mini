using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Mao.Web.Database.Models
{
    public class AppUser
    {
        [Key]
        public Guid Id { get; set; }

        public string Account { get; set; }
        public string PasswordHash { get; set; }

        public string DisplayName { get; set; }

        public string Email { get; set; }
        public bool EmailConfirmed { get; set; }

        public string PhoneNumber { get; set; }
        public bool PhoneNumberConfirmed { get; set; }
    }
}