using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;

namespace Mao.Web.Features.Attributes
{
    /// <summary>
    /// 設置在 ApiController 或 ApiController 的方法上，讓方法執行前驗證 ModelState.IsValid
    /// </summary>
    public class ValidateModelAttribute : ActionFilterAttribute
    {
        /// <summary>
        /// 是否驗證 ModelState
        /// </summary>
        public bool IsValidateModelState { get; set; } = true;

        public override void OnActionExecuting(HttpActionContext actionContext)
        {
            if (IsValidateModelState)
            {
                if (actionContext.ActionDescriptor.GetCustomAttributes<ValidateModelAttribute>()
                    .Any(x => x.IsValidateModelState == false) == false)
                {
                    if (!actionContext.ModelState.IsValid)
                    {
                        actionContext.Response = actionContext.Request.CreateErrorResponse(HttpStatusCode.BadRequest, actionContext.ModelState);
                    }
                }
            }
        }
    }
}