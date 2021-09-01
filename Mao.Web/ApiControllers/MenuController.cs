using Mao.Web.ApiActions;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;

namespace Mao.Web.ApiControllers
{
    [RoutePrefix("api/Menu")]
    public class MenuController : ApiController
    {
        private readonly IMediator _mediator;
        public MenuController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet, Route]
        public async Task<IHttpActionResult> Get([FromUri] GetMenu.Request request)
        {
            var response = await _mediator.Send(request);
            return Ok(response);
        }

        [HttpGet, Route("List")]
        public async Task<IHttpActionResult> GetList([FromUri] GetMenuList.Request request)
        {
            var response = await _mediator.Send(request ?? new GetMenuList.Request());
            return Ok(response);
        }

        [HttpPost, Route]
        public async Task<IHttpActionResult> Add(AddMenu.Request request)
        {
            var response = await _mediator.Send(request);
            return Ok(response);
        }

        [HttpPut, Route]
        public async Task<IHttpActionResult> Update(UpdateMenu.Request request)
        {
            var response = await _mediator.Send(request);
            return Ok(response);
        }

        [HttpDelete, Route]
        public async Task<IHttpActionResult> Delete(DeleteMenu.Request request)
        {
            var response = await _mediator.Send(request);
            return Ok(response);
        }
    }
}
