using Mao.Generate;
using Mao.Generate.Models;
using Mao.Web.ApiActions;
using Mao.Web.Areas.Generate.Views.Generate.MaoWebMini.TableManage;
using Mao.Web.Features.Interfaces;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Mao.Web.Features.Generators.MaoWebMini.TableManage.ApiControllers
{
    public class Controller : IGenerator<Input>
    {
        public GenerateOutputFiles.Response.Files Generate(Input input)
        {
            var content = $@"
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
{{
    [RoutePrefix(""api/{input.TableAlias}"")]
    public class {input.TableAlias.ToUpperCamelCase()}Controller : ApiController
    {{
        private readonly IMediator _mediator;
        public {input.TableAlias.ToUpperCamelCase()}Controller(IMediator mediator)
        {{
            _mediator = mediator;
        }}

        [HttpGet, Route]
        public async Task<IHttpActionResult> Get([FromUri] Get{input.TableAlias.ToUpperCamelCase()}.Request request)
        {{
            var response = await _mediator.Send(request);
            return Ok(response);
        }}

        [HttpGet, Route(""List"")]
        public async Task<IHttpActionResult> GetList([FromUri] Get{input.TableAlias.ToUpperCamelCase()}List.Request request)
        {{
            if (request == null)
            {{
                request = new Get{input.TableAlias.ToUpperCamelCase()}List.Request();
            }}
            var response = await _mediator.Send(request);
            return Ok(response);
        }}

        [HttpPost, Route]
        public async Task<IHttpActionResult> Add(Add{input.TableAlias.ToUpperCamelCase()}.Request request)
        {{
            var response = await _mediator.Send(request);
            return Ok(response);
        }}

        [HttpPut, Route]
        public async Task<IHttpActionResult> Update(Update{input.TableAlias.ToUpperCamelCase()}.Request request)
        {{
            var response = await _mediator.Send(request);
            return Ok(response);
        }}

        [HttpDelete, Route]
        public async Task<IHttpActionResult> Delete(Delete{input.TableAlias.ToUpperCamelCase()}.Request request)
        {{
            var response = await _mediator.Send(request);
            return Ok(response);
        }}
    }}
}}
".TrimStart('\r', '\n');
            return new GenerateOutputFiles.Response.File()
            {
                Name = $"{input.TableAlias.ToUpperCamelCase()}Controller.cs",
                Content = content
            };
        }
    }
}