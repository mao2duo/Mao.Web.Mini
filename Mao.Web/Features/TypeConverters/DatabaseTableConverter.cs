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
    /// DatabaseTable 與 SqlTable 互相轉換
    /// </summary>
    public class DatabaseTableConverter : TypeConverter
    {
        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            return typeof(SqlTable).IsAssignableFrom(sourceType);
        }

        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            if (value is SqlTable sqlTable)
            {
                DatabaseTable table = new DatabaseTable();
                table.TableName = sqlTable.Name;
                table.Description = sqlTable.Description;
                table.Columns = sqlTable.Columns?
                    .OrderBy(x => x.Order)
                    .Select(x => ObjectResolver.TypeConvert<DatabaseTableColumn>(x))
                    .ToList();
                return table;
            }
            return base.ConvertFrom(context, culture, value);
        }

        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
        {
            return destinationType.IsAssignableFrom(typeof(SqlTable));
        }

        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            if (value is DatabaseTable table)
            {
                SqlTable sqlTable = new SqlTable();
                sqlTable.Name = table.TableName;
                sqlTable.Description = table.Description;
                sqlTable.Columns = table.Columns?.Select(x => ObjectResolver.TypeConvert<SqlColumn>(x)).ToArray();
                return sqlTable;
            }
            return base.ConvertTo(context, culture, value, destinationType);
        }
    }
}