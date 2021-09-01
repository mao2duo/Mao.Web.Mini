using Mao.Web.ApiActions.User;
using Mao.Web.Features.Attributes;
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
    [RoutePrefix("api/User")]
    public class UserController : ApiController
    {
        private readonly IMediator _mediator;
        public UserController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost, Route("Register"), ValidateModel]
        public async Task<IHttpActionResult> Register(RegisterUser.Request request)
        {
            var response = _mediator.Send(request);
            return Ok(response);
        }

        [HttpGet, Route("Claims")]
        public async Task<IHttpActionResult> Claims()
        {
            var claimDictionary = User.Identity.GetClaims()?.ToDictionary(x => x.Type, x => x.Value);
            return Ok(claimDictionary);
        }
    }
}
