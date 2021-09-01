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
    [RoutePrefix("api/Database"), Authorize]
    public class DatabaseController : ApiController
    {
        private readonly IMediator _mediator;
        public DatabaseController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet, Route]
        public async Task<IHttpActionResult> Get([FromUri] GetDatabase.Request request)
        {
            var response = await _mediator.Send(request);
            return Ok(response);
        }

        [HttpGet, Route("List")]
        public async Task<IHttpActionResult> GetList([FromUri] GetDatabaseList.Request request)
        {
            if (request == null)
            {
                request = new GetDatabaseList.Request();
            }
            request.UserId = User.Identity.GetClaimValue(ClaimTypes.Sid).ToGuid();
            var response = await _mediator.Send(request);
            return Ok(response);
        }

        [HttpPost, Route]
        public async Task<IHttpActionResult> Add(AddDatabase.Request request)
        {
            request.Database.UserId = User.Identity.GetClaimValue(ClaimTypes.Sid).ToGuid();
            var response = await _mediator.Send(request);
            return Ok(response);
        }

        [HttpPut, Route]
        public async Task<IHttpActionResult> Update(UpdateDatabase.Request request)
        {
            var response = await _mediator.Send(request);
            return Ok(response);
        }

        [HttpDelete, Route]
        public async Task<IHttpActionResult> Delete(DeleteDatabase.Request request)
        {
            var response = await _mediator.Send(request);
            return Ok(response);
        }
    }
}
