using Mao.Web.ApiActions;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web.Http;

namespace Mao.Web.ApiControllers
{
    [RoutePrefix("api/GenerateInput"), Authorize]
    public class GenerateInputController : ApiController
    {
        private readonly IMediator _mediator;
        public GenerateInputController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet, Route]
        public async Task<IHttpActionResult> Get([FromUri] GetGenerateInput.Request request)
        {
            var response = await _mediator.Send(request);
            return Ok(response);
        }

        [HttpGet, Route("List")]
        public async Task<IHttpActionResult> GetList([FromUri] GetGenerateInputList.Request request)
        {
            if (request == null)
            {
                request = new GetGenerateInputList.Request();
            }
            request.UserId = User.Identity.GetClaimValue(ClaimTypes.Sid).ToGuid();
            var response = await _mediator.Send(request);
            return Ok(response);
        }

        [HttpPost, Route]
        public async Task<IHttpActionResult> Add(AddGenerateInput.Request request)
        {
            request.Input.UserId = User.Identity.GetClaimValue(ClaimTypes.Sid).ToGuid();
            var response = await _mediator.Send(request);
            return Ok(response);
        }

        [HttpPut, Route]
        public async Task<IHttpActionResult> Update(UpdateGenerateInput.Request request)
        {
            var response = await _mediator.Send(request);
            return Ok(response);
        }

        [HttpDelete, Route]
        public async Task<IHttpActionResult> Delete(DeleteGenerateInput.Request request)
        {
            var response = await _mediator.Send(request);
            return Ok(response);
        }
    }
}
