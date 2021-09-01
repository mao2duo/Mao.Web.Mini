using Mao.Web.Database.Models;
using Mao.Web.Features.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Html;
using System.Web.Routing;

namespace Mao.Web.Extensions
{
    public static class HtmlHelperExtension
    {
        /// <summary>
        /// 依照條件決定顯示的 HTML
        /// </summary>
        public static IHtmlString ShowHtmlIf(this HtmlHelper helper, bool filter, string value1, string value2 = null)
        {
            if (filter)
            {
                return new HtmlString(value1);
            }
            if (!string.IsNullOrEmpty(value2))
            {
                return new HtmlString(value2);
            }
            return null;
        }
        /// <summary>
        /// 依照條件決定顯示的文字
        /// </summary>
        public static string ShowTextIf(this HtmlHelper helper, bool filter, string value1, string value2 = null)
        {
            if (filter)
            {
                return value1;
            }
            return value2;
        }

        /// <summary>
        /// 文字加密
        /// <para>HtmlHelper 的擴充方法不提供解密，解密不應該在檢視層執行</para>
        /// </summary>
        public static string Encrypt(this HtmlHelper helper, string plaintext)
        {
            //var encryptor = helper.ViewContext.HttpContext.GetService(typeof(IEncryptor)) as IEncryptor;
            var encryptor = DependencyResolver.Current.GetService<IEncryptor>();
            return encryptor.Encrypt(plaintext);
        }

        /// <summary>
        /// 取得 AppMenu 包含 Icon 的 InnerHTML
        /// </summary>
        public static IHtmlString MenuInnerHtml(this HtmlHelper helper, AppMenu menu)
        {
            return new HtmlString($"{Invoker.If(!string.IsNullOrEmpty(menu.Icon), () => $@"<i class=""{menu.Icon}""></i>")}{helper.Encode(menu.Text)}");
        }
    }
}