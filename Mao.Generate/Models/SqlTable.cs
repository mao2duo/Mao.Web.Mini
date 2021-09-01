﻿using Mao.Generate.TypeConverters;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mao.Generate.Models
{
    [TypeConverter(typeof(SqlTableConverter))]
    public class SqlTable
    {
        /// <summary>
        /// 資料表名稱
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 資料表描述
        /// </summary>
        public string Description { get; set; }
        /// <summary>
        /// 資料表的資料行
        /// </summary>
        public SqlColumn[] Columns { get; set; }
    }
}
