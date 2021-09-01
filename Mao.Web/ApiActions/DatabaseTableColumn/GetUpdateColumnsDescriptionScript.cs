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
    /// 取得更新資料欄位描述的 SQL 語法
    /// </summary>
    public class GetUpdateColumnsDescriptionScript
    {
        public class Request : IRequest<Response>
        {
            public Guid DatabaseId { get; set; }
            [Required]
            public string TableName { get; set; }
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
                var databaseTableColumns = _repository.Select<DatabaseTableColumn>(new Dictionary<string, object>()
                {
                    { "DatabaseId", request.DatabaseId },
                    { "TableName", request.TableName }
                });
                switch (request.DbProvider)
                {
                    case "SqlServer":
                        response.Script = databaseTableColumns
                            .Select(GetSqlServerScript)
                            .Join()
                            .TrimStart('\r', '\n');
                        break;
                    default:
                        throw new NotSupportedException($"目前未提供 {request.DbProvider} 更新資料欄位描述的語法");
                }
                return response;
            }

            private string GetSqlServerScript(DatabaseTableColumn column)
            {
                return $@"
-- {column.TableName}.{column.ColumnName}
IF NOT EXISTS (SELECT *
               FROM   sys.objects o
                      INNER JOIN sys.columns c ON c.object_id = o.object_id
                      INNER JOIN sys.extended_properties o_des ON o_des.major_id = o.object_id AND o_des.minor_id = c.column_id AND o_des.[name] = 'MS_Description'
               WHERE  o.type = 'U' AND o.[name] = '{column.TableName.Replace("'", "''")}' AND c.[name] = '{column.ColumnName.Replace("'", "''")}')
  BEGIN
      EXEC sp_addextendedproperty
        @name = N'MS_Description',
        @value = N'{(column.Description ?? "").Replace("'", "''")}',
        @level0type = N'SCHEMA',
        @level0name = N'dbo',
        @level1type = N'TABLE',
        @level1name = N'{column.TableName.Replace("'", "''")}',
        @level2type = N'COLUMN',
        @level2name = N'{column.ColumnName.Replace("'", "''")}'
  END
ELSE
  BEGIN
      EXEC sp_updateextendedproperty
        @name = N'MS_Description',
        @value = N'{(column.Description ?? "").Replace("'", "''")}',
        @level0type = N'SCHEMA',
        @level0name = N'dbo',
        @level1type = N'TABLE',
        @level1name = N'{column.TableName.Replace("'", "''")}',
        @level2type = N'COLUMN',
        @level2name = N'{column.ColumnName.Replace("'", "''")}'
  END 
";
            }
        }
    }
}