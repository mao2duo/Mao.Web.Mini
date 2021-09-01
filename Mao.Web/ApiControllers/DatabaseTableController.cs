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
    [RoutePrefix("api/DatabaseTable")]
    public class DatabaseTableController : ApiController
    {
        private readonly IMediator _mediator;
        public DatabaseTableController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet, Route]
        public async Task<IHttpActionResult> Get([FromUri] GetDatabaseTable.Request request)
        {
            var response = await _mediator.Send(request);
            return Ok(response);
        }

        [HttpGet, Route("List")]
        public async Task<IHttpActionResult> GetList([FromUri] GetDatabaseTableList.Request request)
        {
            var response = await _mediator.Send(request);
            return Ok(response);
        }

        [HttpPost, Route]
        public async Task<IHttpActionResult> Add(AddDatabaseTable.Request request)
        {
            var response = await _mediator.Send(request);
            return Ok(response);
        }

        [HttpPut, Route]
        public async Task<IHttpActionResult> Update(UpdateDatabaseTable.Request request)
        {
            var response = await _mediator.Send(request);
            return Ok(response);
        }

        [HttpPut, Route("List")]
        public async Task<IHttpActionResult> UpdateList(UpdateDatabaseTableList.Request request)
        {
            var response = await _mediator.Send(request);
            return Ok(response);
        }

        [HttpDelete, Route]
        public async Task<IHttpActionResult> Delete(DeleteDatabaseTable.Request request)
        {
            var response = await _mediator.Send(request);
            return Ok(response);
        }

        [HttpPost, Route("ConvertToSqlTablesSerialized")]
        public async Task<IHttpActionResult> ConvertToSqlTablesSerialized(ConvertToSqlTablesSerialized.Request request)
        {
            if (request == null)
            {
                request = new ConvertToSqlTablesSerialized.Request();
            }
            if (request.Tables == null)
            {
                request.Tables = new DatabaseTable[0];
            }
            var response = await _mediator.Send(request);
            return Ok(response);
        }

        [HttpPost, Route("ConvertFromSqlTablesSerialized")]
        public async Task<IHttpActionResult> ConvertFromSqlTablesSerialized(ConvertFromSqlTablesSerialized.Request request)
        {
            var response = await _mediator.Send(request);
            return Ok(response);
        }

        [HttpGet, Route("SerializeSqlTablesScript")]
        public async Task<IHttpActionResult> GetSerializeSqlTablesScript([FromUri] GetSerializeSqlTablesScript.Request request)
        {
            var response = await _mediator.Send(request);
            return Ok(response);
        }

        [HttpGet, Route("UpdateTablesDescScript")]
        public async Task<IHttpActionResult> GetUpdateTablesDescriptionScript([FromUri] GetUpdateTablesDescriptionScript.Request request)
        {
            var response = await _mediator.Send(request);
            return Ok(response);
        }
    }
}
