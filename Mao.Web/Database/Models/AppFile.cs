using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Mao.Web.Database.Models
{
    public class AppFile
    {
        [Key]
        public Guid Id { get; set; }
        /// <summary>
        /// 上傳時間
        /// </summary>
        public DateTime CreatedAt { get; set; }
        /// <summary>
        /// 上傳會員
        /// </summary>
        public Guid? CreatedBy { get; set; }
        /// <summary>
        /// 縮圖 (Base64)
        /// </summary>
        public string Thumbnail { get; set; }
        /// <summary>
        /// 檔案名稱 (含副檔名)
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 檔案內容 (Base64)
        /// </summary>
        public string Content { get; set; }
        /// <summary>
        /// 檔案類型
        /// </summary>
        public string ContentType { get; set; }
        /// <summary>
        /// 檔案大小 (B)
        /// </summary>
        public int Size { get; set; }
        /// <summary>
        /// 是否為暫存狀態
        /// </summary>
        public bool IsTemp { get; set; }
        /// <summary>
        /// 是否為一次性檔案
        /// </summary>
        public bool IsDisposable { get; set; }
    }
}