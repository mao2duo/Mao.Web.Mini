using Mao.Web.ApiActions;
using Mao.Web.Database.Models;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.ModelBinding;

namespace Mao.Web.ApiControllers
{
    [RoutePrefix("api/Generate")]
    public class GenerateController : ApiController
    {
        private readonly IMediator _mediator;
        public GenerateController(IMediator mediator)
        {
            _mediator = mediator;
        }

        /// <summary>
        /// 根據程式範本參數輸出結果
        /// </summary>
        [HttpPost, Route("Output")]
        public async Task<IHttpActionResult> Output([ModelBinder] GenerateOutputFiles.Request request)
        {
            var response = await _mediator.Send(request);
            return Ok(response);
        }
        /// <summary>
        /// 將程式範本參數的輸出結果壓縮成 ZIP 檔案
        /// </summary>
        [HttpPost, Route("Zip")]
        public async Task<IHttpActionResult> Zip(ZipOutputFiles.Request request)
        {
            var zip = await _mediator.Send(request);
            var response = await _mediator.Send(new AddFile.Request()
            {
                File = new AppFile()
                {
                    Name = request.Name,
                    Content = Convert.ToBase64String(zip.Bytes),
                    ContentType = "application/zip",
                    Size = zip.Bytes.Length,
                    CreatedBy = User.Identity.GetClaimValue(ClaimTypes.Sid).ToGuid(),
                    IsTemp = true,
                    IsDisposable = true
                }
            });
            return Ok(response);
        }
    }
}
