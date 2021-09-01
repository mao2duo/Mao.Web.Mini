using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;

namespace Mao.Web.ApiControllers
{
    [RoutePrefix("api/Develop")]
    public class DevelopController : ApiController
    {
        [HttpGet, Route("Exception")]
        public async Task<IHttpActionResult> Exception()
        {
            throw new Exception("Test");
        }
    }
}
