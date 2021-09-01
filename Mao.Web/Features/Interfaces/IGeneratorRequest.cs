using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mao.Web.Features.Interfaces
{
    /// <summary>
    /// 繼承這個介面可以處理 UpdateModel 以外的需求
    /// </summary>
    public interface IGeneratorRequest
    {
        /// <summary>
        /// 是否將所有字串預設為空字串而非 null
        /// </summary>
        bool UseDefaultStringEmpty { get; }
        /// <summary>
        /// 是否將所有 class 的屬性預設為 new() 而非 null
        /// </summary>
        bool UseDefaultInstance { get; }
        /// <summary>
        /// 是否將所有集合預設為空集合而非 null
        /// </summary>
        bool UseDefaultEmptyCollection { get; }

        /// <summary>
        /// 是否套用 UpdateModel
        /// </summary>
        bool UseUpdateModel { get; }

        /// <summary>
        /// 取得 Request.Form 來更新這個物件
        /// </summary>
        void ReceiveRequestForm(NameValueCollection form);

        /// <summary>
        /// 在物件更新後執行的方法
        /// </summary>
        void OnAfterUpdateModel();
    }
}
