using Mao.Web.ApiActions;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace Mao.Web.Controllers
{
    public class FileController : Controller
    {
        private readonly IMediator _mediator;
        public FileController(IMediator mediator)
        {
            _mediator = mediator;
        }

        /// <summary>
        /// 下載檔案
        /// </summary>
        public async Task<ActionResult> Download(string id)
        {
            var response = await _mediator.Send(new GetFile.Request()
            {
                Id = new Guid(id)
            });
            if (string.IsNullOrEmpty(response.Content))
            {
                return HttpNotFound();
            }
            return File(Convert.FromBase64String(response.Content), response.ContentType, response.Name);
        }
    }
}