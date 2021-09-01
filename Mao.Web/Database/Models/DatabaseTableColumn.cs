using Mao.Generate.Models;
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
    [TypeConverter(typeof(DatabaseTableColumnConverter))]
    public class DatabaseTableColumn
    {
        [Key]
        public Guid DatabaseId { get; set; }
        [Key]
        public string TableName { get; set; }
        [Key]
        public string ColumnName { get; set; }

        public string TypeFullName { get; set; }
        public bool IsNullable { get; set; }

        public bool IsPrimaryKey { get; set; }
        public bool IsIdentity { get; set; }
        public bool IsComputed { get; set; }

        public string DefaultDefine { get; set; }
        /// <summary>
        /// 描述
        /// </summary>
        public string Description { get; set; }

        public int Sort { get; set; }
    }
}