using Mao.Web.Features.TypeConverters;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Mao.Web.Database.Models
{
    [TypeConverter(typeof(DatabaseTableConverter))]
    public class DatabaseTable
    {
        [Key]
        public Guid DatabaseId { get; set; }
        [Key]
        public string TableName { get; set; }

        public string TableAlias { get; set; }

        public string Description { get; set; }

        [NotMapped]
        public ICollection<DatabaseTableColumn> Columns { get; set; }
    }
}