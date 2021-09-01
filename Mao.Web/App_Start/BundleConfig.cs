using System.Web;
using System.Web.Optimization;

namespace Mao.Web
{
    public class BundleConfig
    {
        // 如需統合的詳細資訊，請瀏覽 https://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new StyleBundle("~/lib/css").Include(
                "~/lib/fontawesome-free-5.15.3-web/css/all.min.css",                                                // fontawesome
                "~/lib/bootstrap-4.6.0-dist/css/bootstrap.min.css",                                                 // bootstrap
                "~/lib/bootstrap-datepicker-1.9.0-dist/css/bootstrap-datepicker.min.css",                           // bootstrap-datepicker
                "~/lib/bootstrap-table-1.18.3-dist/bootstrap-table.min.css",                                        // bootstrap-table
                "~/lib/bootstrap-table-1.18.3-dist/extensions/reorder-rows/bootstrap-table-reorder-rows.min.css",   // * bootstrap-table 拖拉排序套件
                "~/lib/site.css"));

            bundles.Add(new ScriptBundle("~/lib/js").Include(
                "~/lib/jquery-3.6.0.min.js",                                                                        // jquery
                "~/lib/jquery-deserialize-2.0.0-rc1/jquery.deserialize.js",                                         // jquery.deserialize
                "~/lib/popper.js-1.16.1/umd/popper.min.js",                                                         // * bootstrap 下拉選單功能需要的參考
                "~/lib/fontawesome-free-5.15.3-web/js/all.min.js",                                                  // fontawesome
                "~/lib/bootstrap-4.6.0-dist/js/bootstrap.min.js",                                                   // bootstrap
                "~/lib/bootstrap-datepicker-1.9.0-dist/js/bootstrap-datepicker.min.js",                             // bootstrap-datepicker
                "~/lib/bootstrap-table-1.18.3-dist/bootstrap-table.min.js",                                         // bootstrap-table
                "~/lib/bootstrap-table-1.18.3-dist/extensions/reorder-rows/bootstrap-table-reorder-rows.min.js",    // * bootstrap-table 拖拉排序套件
                "~/lib/ckeditor-4.16.1-full-easyimage/ckeditor.js",                                                 // ckeditor
                "~/lib/mustache-4.2.0/mustache.js",                                                                 // mustache
                "~/lib/site.js"));
        }
    }
}
