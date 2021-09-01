using Mao.Web.Features.Interfaces;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Web;

namespace Mao.Web.Areas.Generate.Views.Generate.MaoWebMini.TableManage
{
    public class Input
    {
        public string ProjectName { get; set; }
        public string TableName { get; set; }
        public string TableAlias { get; set; }
        public string TableDescription { get; set; }
        public string TableColumnsJson { get; set; }
        public string AddAndUpdateView { get; set; }
        public TableColumnGenerateSettings[] TableColumnGenerateSettings { get; set; }
    }

    public class TableColumnGenerateSettings
    {
        public string ColumnName { get; set; }
        public string InputType { get; set; }
        public string[] Validation { get; set; }
        public TableColumnGenerateSettingsOnList OnList { get; set; }
        public TableColumnGenerateSettingsOnCreate OnCreate { get; set; }
        public TableColumnGenerateSettingsOnUpdate OnUpdate { get; set; }
    }

    public class TableColumnGenerateSettingsOnList
    {
        public string[] Features { get; set; }
    }

    public class TableColumnGenerateSettingsOnCreate
    {
        public string InputStatus { get; set; }
    }

    public class TableColumnGenerateSettingsOnUpdate
    {
        public string InputStatus { get; set; }
    }
}