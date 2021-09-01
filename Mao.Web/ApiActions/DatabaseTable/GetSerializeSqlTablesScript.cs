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
    public class GetSerializeSqlTablesScript
    {
        public class Request : IRequest<Response>
        {
            [Required]
            public string DbProvider { get; set; }
        }

        public class Response
        {
            public string Script { get; set; }
        }

        public class Handler : IRequestHandler<Request, Response>
        {
            public async Task<Response> Handle(Request request, CancellationToken cancellationToken)
            {
                Response response = new Response();
                switch (request.DbProvider)
                {
                    case "SqlServer":
                        response.Script = GetSqlServerScript();
                        break;
                    default:
                        throw new NotSupportedException($"目前未提供 {request.DbProvider} 序列化資料表的語法");
                }
                return response;
            }

            private string GetSqlServerScript()
            {
                return @"
-- XML 格式 (適用於 Sql Server 2008 以上)
SELECT o.[name]                       AS [Name],
       o_des.[value]                  AS [Description],
       (SELECT c.column_id                            AS Id,
               CASE
                 WHEN EXISTS (SELECT *
                              FROM   sys.index_columns AS ic
                                     LEFT JOIN sys.indexes i ON i.object_id = ic.object_id AND i.index_id = ic.index_id
                              WHERE  ic.column_id = c.column_id AND i.object_id = c.object_id AND i.is_primary_key = 1) THEN 1
                 ELSE 0
               END                                    AS IsPrimaryKey,
               c.[name]                               AS [Name],
               t.[name]                               AS TypeName,
               sc.prec                                AS [Length],
               c.[precision]                          AS Prec,
               c.scale                                AS Scale,
               c.is_nullable                          AS IsNullable,
               c.is_identity                          AS IsIdentity,
               c.is_computed                          AS IsComputed,
               Object_definition(c.default_object_id) AS DefaultDefine,
               p_des.value                            AS [Description],
               sc.colorder                            AS [Order]
        FROM   sys.columns c
               INNER JOIN syscolumns sc ON c.object_id = sc.id AND c.column_id = sc.colid
               LEFT JOIN sys.types t ON t.user_type_id = c.user_type_id
               LEFT JOIN sys.extended_properties p_des ON c.object_id = p_des.major_id AND c.column_id = p_des.minor_id AND p_des.[name] = 'MS_Description'
        WHERE  c.object_id = o.object_id
        ORDER  BY sc.colorder
        FOR XML PATH('column'), TYPE) AS [columns]
FROM   sys.objects o
       LEFT JOIN sys.extended_properties o_des ON o_des.major_id = o.object_id AND o_des.minor_id = 0 AND o_des.[name] = 'MS_Description'
WHERE  o.type = 'U'
ORDER  BY o.[name]
FOR XML PATH('table'), ROOT('tables') 

-- JSON 格式 (適用於 Sql Server 2016 以上)
SELECT o.[name]                       AS [Name],
       o_des.[value]                  AS [Description],
       (SELECT c.column_id                            AS Id,
               CASE
                 WHEN EXISTS (SELECT *
                              FROM   sys.index_columns AS ic
                                     LEFT JOIN sys.indexes i ON i.object_id = ic.object_id AND i.index_id = ic.index_id
                              WHERE  ic.column_id = c.column_id AND i.object_id = c.object_id AND i.is_primary_key = 1) THEN 1
                 ELSE 0
               END                                    AS IsPrimaryKey,
               c.[name]                               AS [Name],
               t.[name]                               AS TypeName,
               sc.prec                                AS [Length],
               c.[precision]                          AS Prec,
               c.scale                                AS Scale,
               c.is_nullable                          AS IsNullable,
               c.is_identity                          AS IsIdentity,
               c.is_computed                          AS IsComputed,
               Object_definition(c.default_object_id) AS DefaultDefine,
               p_des.value                            AS [Description],
               sc.colorder                            AS [Order]
        FROM   sys.columns c
               INNER JOIN syscolumns sc ON c.object_id = sc.id AND c.column_id = sc.colid
               LEFT JOIN sys.types t ON t.user_type_id = c.user_type_id
               LEFT JOIN sys.extended_properties p_des ON c.object_id = p_des.major_id AND c.column_id = p_des.minor_id AND p_des.[name] = 'MS_Description'
        WHERE  c.object_id = o.object_id
        ORDER  BY sc.colorder
        FOR JSON PATH) AS [columns]
FROM   sys.objects o
       LEFT JOIN sys.extended_properties o_des ON o_des.major_id = o.object_id AND o_des.minor_id = 0 AND o_des.[name] = 'MS_Description'
WHERE  o.type = 'U'
ORDER  BY o.[name]
FOR JSON PATH".TrimStart();
            }
        }
    }
}