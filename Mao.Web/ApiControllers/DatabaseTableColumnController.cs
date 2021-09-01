using Mao.Web.ApiActions;
using Mao.Web.Database.Models;
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
    [RoutePrefix("api/DatabaseTableColumn")]
    public class DatabaseTableColumnController : ApiController
    {
        private readonly IMediator _mediator;
        public DatabaseTableColumnController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet, Route]
        public async Task<IHttpActionResult> Get([FromUri] GetDatabaseTableColumn.Request request)
        {
            var response = await _mediator.Send(request);
            return Ok(response);
        }

        [HttpGet, Route("List")]
        public async Task<IHttpActionResult> GetList([FromUri] GetDatabaseTableColumnList.Request request)
        {
            var response = await _mediator.Send(request);
            return Ok(response);
        }

        [HttpPost, Route]
        public async Task<IHttpActionResult> Add(AddDatabaseTableColumn.Request request)
        {
            var response = await _mediator.Send(request);
            return Ok(response);
        }

        [HttpPut, Route]
        public async Task<IHttpActionResult> Update(UpdateDatabaseTableColumn.Request request)
        {
            var response = await _mediator.Send(request);
            return Ok(response);
        }

        [HttpPut, Route("List")]
        public async Task<IHttpActionResult> UpdateList(UpdateDatabaseTableColumnList.Request request)
        {
            var response = await _mediator.Send(request);
            return Ok(response);
        }

        [HttpDelete, Route]
        public async Task<IHttpActionResult> Delete(DeleteDatabaseTableColumn.Request request)
        {
            var response = await _mediator.Send(request);
            return Ok(response);
        }

        [HttpPost, Route("ConvertToSqlColumnsSerialized")]
        public async Task<IHttpActionResult> ConvertToSqlColumnsSerialized(ConvertToSqlColumnsSerialized.Request request)
        {
            if (request == null)
            {
                request = new ConvertToSqlColumnsSerialized.Request();
            }
            if (request.Columns == null)
            {
                request.Columns = new DatabaseTableColumn[0];
            }
            var response = await _mediator.Send(request);
            return Ok(response);
        }

        [HttpPost, Route("ConvertFromSqlColumnsSerialized")]
        public async Task<IHttpActionResult> ConvertFromSqlColumnsSerialized(ConvertFromSqlColumnsSerialized.Request request)
        {
            var response = await _mediator.Send(request);
            return Ok(response);
        }

        [HttpGet, Route("SerializeSqlColumnsScript")]
        public async Task<IHttpActionResult> GetSerializeSqlColumnsScript([FromUri] GetSerializeSqlColumnsScript.Request request)
        {
            var response = await _mediator.Send(request);
            return Ok(response);
        }

        [HttpGet, Route("UpdateColumnsDescScript")]
        public async Task<IHttpActionResult> GetUpdateColumnsDescriptionScript([FromUri] GetUpdateColumnsDescriptionScript.Request request)
        {
            var response = await _mediator.Send(request);
            return Ok(response);
        }
    }
}
