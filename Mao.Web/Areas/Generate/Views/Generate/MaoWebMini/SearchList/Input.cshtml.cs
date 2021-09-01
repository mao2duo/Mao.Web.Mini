using Mao.Web.Features.Interfaces;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Web;

namespace Mao.Web.Areas.Generate.Views.Generate.MaoWebMini.SearchList
{
    public class Input : IGeneratorRequest
    {
        public string ProjectName { get; set; }
        public string ControllerName { get; set; }
        public string ViewName { get; set; }
        public string ParentFunctionNumber { get; set; }
        public string FunctionNumber { get; set; }
        public string PageTitle { get; set; }
        public bool HasToExcel { get; set; }
        public bool IsShowRowNumber { get; set; }

        public SearchInputSettings[] SearchInputSettings { get; set; }
        public ListColumnSettings[] ListColumnSettings { get; set; }

        public bool UseDefaultStringEmpty => true;
        public bool UseDefaultInstance => true;
        public bool UseDefaultEmptyCollection => true;
        public bool UseUpdateModel => true;

        public void ReceiveRequestForm(NameValueCollection form)
        {
            throw new NotImplementedException();
        }
        public void OnAfterUpdateModel()
        {
            throw new NotImplementedException();
        }
    }

    public class SearchInputSettings
    {
        public string DisplayName { get; set; }
        public string ParameterName { get; set; }
        public string InputType { get; set; }
        public string DataType { get; set; }
        public string OnLoad { get; set; }
        public string[] Validation { get; set; }
    }

    public class ListColumnSettings
    {
        public string ColumnTitle { get; set; }
        public string ColumnName { get; set; }
        public string DataType { get; set; }
        public bool HasFormatter { get; set; }
    }
}