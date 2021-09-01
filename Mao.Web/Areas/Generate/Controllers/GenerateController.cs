using Mao.Web.ApiActions;
using Mao.Web.Database.Models;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace Mao.Web.Areas.Generate.Controllers
{
    public class GenerateController : GenerateAreaController
    {
        private readonly IMediator _mediator;
        public GenerateController(IMediator mediator)
        {
            _mediator = mediator;
        }

        // GET: Generate
        public ActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// 載入對應的程式範本參數輸入頁面
        /// </summary>
        public ActionResult Input(string provider, string module)
        {
            ViewBag.Provider = provider;
            ViewBag.Module = module;
            return View($"{provider}/{module}/Input");
        }
        /// <summary>
        /// 根據程式範本參數輸出結果
        /// </summary>
        [HttpPost]
        public async Task<ActionResult> Output()
        {
            GenerateOutputFiles.Request request = new GenerateOutputFiles.Request();
            request.Provider = Request.QueryString.Get("Provider");
            request.Module = Request.QueryString.Get("Module");
            var updateModel = this.GetType()
                .GetMethods(BindingFlags.Instance | BindingFlags.NonPublic)
                .First(y => y.Name == "TryUpdateModel" && y.GetParameters().Length == 1);
            request.UpdateModel = x =>
            {
                bool isSuccessed = (bool)updateModel
                    .MakeGenericMethod(x.GetType())
                    .Invoke(this, new object[] { x });
                if (!isSuccessed)
                {
                    // TODO: merge ModelState
                }
            };
            var response = await _mediator.Send(request);
            if (!this.ModelState.IsValid)
            {
                Response.StatusCode = 400;
                Response.StatusDescription = "Bad Request - Model State is Invalid";
                //Response.TrySkipIisCustomErrors = true;
                return Json(new
                {
                    ModelState = ModelState
                        .Where(x => x.Value.Errors != null && x.Value.Errors.Any())
                        .ToDictionary(
                            x => x.Key,
                            x => x.Value.Errors.Select(y => y.ErrorMessage).ToArray())
                });
            }
            return View(response);
        }
    }
}