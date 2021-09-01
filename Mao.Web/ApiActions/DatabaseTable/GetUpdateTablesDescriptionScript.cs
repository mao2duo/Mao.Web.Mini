using Mao.Repository;
using Mao.Web.Database.Models;
using MediatR;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Web;

namespace Mao.Web.ApiActions
{
    /// <summary>
    /// 取得更新資料表描述的 SQL 語法
    /// </summary>
    public class GetUpdateTablesDescriptionScript
    {
        public class Request : IRequest<Response>
        {
            public Guid DatabaseId { get; set; }
            [Required]
            public string DbProvider { get; set; }
        }

        public class Response
        {
            public string Script { get; set; }
        }

        public class Handler : IRequestHandler<Request, Response>
        {
            private readonly IRepository _repository;
            public Handler(IRepository repository)
            {
                _repository = repository;
            }

            public async Task<Response> Handle(Request request, CancellationToken cancellationToken)
            {
                Response response = new Response();
                var databaseTables = _repository.Select<DatabaseTable>(new Dictionary<string, object>()
                {
                    { "DatabaseId", request.DatabaseId }
                });
                switch (request.DbProvider)
                {
                    case "SqlServer":
                        response.Script = databaseTables
                            .Select(GetSqlServerScript)
                            .Join()
                            .TrimStart('\r', '\n');
                        break;
                    default:
                        throw new NotSupportedException($"目前未提供 {request.DbProvider} 更新資料表描述的語法");
                }
                return response;
            }

            private string GetSqlServerScript(DatabaseTable table)
            {
                return $@"
-- {table.TableName}
IF NOT EXISTS (SELECT *
               FROM   sys.objects o
                      INNER JOIN sys.extended_properties o_des ON o_des.major_id = o.object_id AND o_des.minor_id = 0 AND o_des.[name] = 'MS_Description'
               WHERE  o.type = 'U' AND o.[name] = '{table.TableName.Replace("'", "''")}')
  BEGIN
      EXEC Sp_addextendedproperty
        @name = N'MS_Description',
        @value = N'{(table.Description ?? "").Replace("'", "''")}',
        @level0type = N'SCHEMA',
        @level0name = N'dbo',
        @level1type = N'TABLE',
        @level1name = N'{table.TableName.Replace("'", "''")}',
        @level2type = NULL,
        @level2name = NULL
  END
ELSE
  BEGIN
      EXEC Sp_updateextendedproperty
        @name = N'MS_Description',
        @value = N'{(table.Description ?? "").Replace("'", "''")}',
        @level0type = N'SCHEMA',
        @level0name = N'dbo',
        @level1type = N'TABLE',
        @level1name = N'{table.TableName.Replace("'", "''")}',
        @level2type = NULL,
        @level2name = NULL
  END 
";
            }
        }
    }
}