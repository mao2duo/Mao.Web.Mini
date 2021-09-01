using Mao.Generate.Models;
using Mao.Web.Database.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Web;

namespace Mao.Web.Features.TypeConverters
{
    /// <summary>
    /// DatabaseTableColumn 與 SqlColumn 互相轉換
    /// </summary>
    public class DatabaseTableColumnConverter : TypeConverter
    {
        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            return typeof(SqlColumn).IsAssignableFrom(sourceType);
        }

        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            if (value is SqlColumn sqlColumn)
            {
                DatabaseTableColumn tableColumn = new DatabaseTableColumn();
                tableColumn.ColumnName = sqlColumn.Name;
                tableColumn.TypeFullName = sqlColumn.TypeFullName;
                tableColumn.IsNullable = sqlColumn.IsNullable;
                tableColumn.IsPrimaryKey = sqlColumn.IsPrimaryKey;
                tableColumn.IsIdentity = sqlColumn.IsIdentity;
                tableColumn.IsComputed = sqlColumn.IsComputed;
                tableColumn.DefaultDefine = sqlColumn.DefaultDefine;
                tableColumn.Description = sqlColumn.Description;
                tableColumn.Sort = sqlColumn.Order;
                return tableColumn;
            }
            return base.ConvertFrom(context, culture, value);
        }

        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
        {
            return destinationType.IsAssignableFrom(typeof(SqlColumn));
        }

        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            if (value is DatabaseTableColumn tableColumn)
            {
                SqlColumn sqlColumn = new SqlColumn();
                sqlColumn.Name = tableColumn.ColumnName;
                sqlColumn.TypeFullName = tableColumn.TypeFullName;
                sqlColumn.IsNullable = tableColumn.IsNullable;
                sqlColumn.IsPrimaryKey = tableColumn.IsPrimaryKey;
                sqlColumn.IsIdentity = tableColumn.IsIdentity;
                sqlColumn.IsComputed = tableColumn.IsComputed;
                sqlColumn.DefaultDefine = tableColumn.DefaultDefine;
                sqlColumn.Description = tableColumn.Description;
                sqlColumn.Order = tableColumn.Sort;
                return sqlColumn;
            }
            return base.ConvertTo(context, culture, value, destinationType);
        }
    }
}