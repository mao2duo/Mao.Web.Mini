using Mao.Web.Features.Interfaces;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Compilation;
using System.Web.Mvc;
using System.Web.WebPages;

namespace Mao.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly IEncryptor _encryptor;
        public HomeController(IEncryptor encryptor)
        {
            _encryptor = encryptor;
        }

        public ActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// UI.render.razorHelper 的接口
        /// </summary>
        [HttpPost]
        public async Task<JsonResult> RazorHepler(string viewPathCiphertext, string helperName, string helperParameters)
        {
            var viewPath = _encryptor.Decrypt(viewPathCiphertext);
            var viewType = BuildManager.GetCompiledType(viewPath);
            if (viewType == null)
            {
                throw new Exception($"無法找到路徑 {viewPath} 對應的檢視");
            }
            var viewHelper = viewType.GetMethod(helperName);
            if (viewHelper == null
                || !typeof(HelperResult).IsAssignableFrom(viewHelper.ReturnType))
            {
                throw new NotImplementedException($"無法在檢視 {viewPath} 找到 Helper {helperName}");
            }
            HelperResult viewHelperResult;
            var viewHelperParameters = viewHelper.GetParameters();
            var viewHelperInvokeParameters = new List<object>();
            using (var stringWriter = new StringWriter())
            {
                var viewInstance = Activator.CreateInstance(viewType) as WebViewPage;
                var viewEngineResult = ViewEngines.Engines.FindPartialView(ControllerContext, viewPath);
                var viewContext = new ViewContext(ControllerContext, viewEngineResult.View, ViewData, TempData, stringWriter);
                viewInstance.ViewContext = viewContext;
                viewInstance.InitHelpers();
                var jArray = Invoker.Try(
                    () => JArray.Parse(helperParameters),
                    () => throw new Exception("無法將參數 helperParameters 反序列化為物件陣列"));
                for (int viewHelperParameterIndex = 0;
                    viewHelperParameterIndex < viewHelperParameters.Length;
                    viewHelperParameterIndex++)
                {
                    object viewHelperInvokeParameter;
                    if (viewHelperParameterIndex < jArray.Count)
                    {
                        viewHelperInvokeParameter = jArray[viewHelperParameterIndex].ToObject(viewHelperParameters[viewHelperParameterIndex].ParameterType);
                    }
                    else if (viewHelperParameters[viewHelperParameterIndex].HasDefaultValue)
                    {
                        viewHelperInvokeParameter = viewHelperParameters[viewHelperParameterIndex].DefaultValue;
                    }
                    else
                    {
                        viewHelperInvokeParameter = ObjectResolver.GetDefaultValue(viewHelperParameters[viewHelperParameterIndex].ParameterType);
                    }
                    viewHelperInvokeParameters.Add(viewHelperInvokeParameter);
                }
                viewHelperResult = viewHelper.Invoke(viewInstance, viewHelperInvokeParameters.ToArray()) as HelperResult;
            }
            return Json(viewHelperResult.ToHtmlString());
        }
    }
}