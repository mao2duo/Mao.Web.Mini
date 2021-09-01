using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Mao.Web.Database.Models
{
    public class AppMenu
    {
        [Key]
        public Guid Id { get; set; }
        public Guid? ParentId { get; set; }

        public string Name { get; set; }
        public string Icon { get; set; }
        public string Text { get; set; }

        public bool IsLink { get; set; }
        public string Href { get; set; }
        public string Target { get; set; }

        public int Sort { get; set; }

        /// <summary>
        /// 是否啟用
        /// </summary>
        public bool IsActivated { get; set; }

        [NotMapped]
        public ICollection<AppMenu> Children { get; set; }

        [NotMapped]
        public ICollection<AppMenuRoute> Routes { get; set; }
    }
}