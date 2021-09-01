using Mao.Generate.Models;
using Mao.Web.Database.Models;
using Mao.Web.Features.JsonConverters;
using MediatR;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Xml;

namespace Mao.Web.ApiActions
{
    public class ConvertFromSqlTablesSerialized
    {
        public class Request : IRequest<Response>
        {
            public string JsonOrXml { get; set; }
        }

        public class Response
        {
            public bool IsSuccessed { get; set; }
            public string Message { get; set; }
            public IEnumerable<DatabaseTable> Tables { get; set; }
        }

        public class Handler : IRequestHandler<Request, Response>
        {
            public async Task<Response> Handle(Request request, CancellationToken cancellationToken)
            {
                Response response = new Response();
                string jsonOrXml = request.JsonOrXml?.Trim();
                if (!string.IsNullOrEmpty(jsonOrXml))
                {
                    try
                    {
                        if (jsonOrXml.StartsWith("<") && jsonOrXml.EndsWith(">"))
                        {
                            response.Tables = FromXml(jsonOrXml);
                            response.IsSuccessed = true;
                        }
                        if ((jsonOrXml.StartsWith("{") && jsonOrXml.EndsWith("}"))
                            || (jsonOrXml.StartsWith("[") && jsonOrXml.EndsWith("]")))
                        {
                            response.Tables = FromJson(jsonOrXml);
                            response.IsSuccessed = true;
                        }
                    }
                    catch (Exception e)
                    {
                        response.Message = e.Message;
                    }
                }
                return response;
            }

            private bool StringToBoolean(string text)
            {
                return text == "1"
                    || text?.ToLower() == "t"
                    || text?.ToLower() == "true";
            }

            private IEnumerable<DatabaseTable> FromXml(string xml)
            {
                List<DatabaseTable> databaseTables = new List<DatabaseTable>();
                XmlDocument doc = new XmlDocument();
                doc.LoadXml(xml);
                var tables = doc.DocumentElement.SelectNodes("table");
                foreach (XmlNode table in tables)
                {
                    SqlTable sqlTable = new SqlTable();
                    Invoker.UsingIf(table.SelectSingleNode("Name"),
                        node => node != null,
                        node => sqlTable.Name = node.InnerText);
                    Invoker.UsingIf(table.SelectSingleNode("Description"),
                        node => node != null,
                        node => sqlTable.Description = node.InnerText);
                    List<SqlColumn> sqlColumns = new List<SqlColumn>();
                    var columns = table.SelectNodes("columns/column");
                    foreach (XmlNode column in columns)
                    {
                        SqlColumn sqlColumn = new SqlColumn();
                        Invoker.UsingIf(column.SelectSingleNode("Name"),
                            node => node != null,
                            node => sqlColumn.Name = node.InnerText);
                        Invoker.UsingIf(column.SelectSingleNode("TypeName"),
                            node => node != null,
                            node => sqlColumn.TypeName = node.InnerText);
                        Invoker.UsingIf(column.SelectSingleNode("IsNullable"),
                            node => node != null,
                            node => sqlColumn.IsNullable = StringToBoolean(node.InnerText));
                        Invoker.UsingIf(column.SelectSingleNode("Length"),
                            node => node != null,
                            node => sqlColumn.Length =
                                node.InnerText?.ToLower() == "max" ? -1 : Convert.ToInt32(node.InnerText));
                        Invoker.UsingIf(column.SelectSingleNode("Prec"),
                            node => node != null,
                            node => sqlColumn.Prec = Convert.ToInt32(node.InnerText));
                        Invoker.UsingIf(column.SelectSingleNode("Scale"),
                            node => node != null,
                            node => sqlColumn.Scale = Convert.ToInt32(node.InnerText));
                        Invoker.UsingIf(column.SelectSingleNode("DefaultDefine"),
                            node => node != null,
                            node => sqlColumn.DefaultDefine = node.InnerText);
                        Invoker.UsingIf(column.SelectSingleNode("IsPrimaryKey"),
                            node => node != null,
                            node => sqlColumn.IsPrimaryKey = StringToBoolean(node.InnerText));
                        Invoker.UsingIf(column.SelectSingleNode("IsIdentity"),
                            node => node != null,
                            node => sqlColumn.IsIdentity = StringToBoolean(node.InnerText));
                        Invoker.UsingIf(column.SelectSingleNode("IsComputed"),
                            node => node != null,
                            node => sqlColumn.IsComputed = StringToBoolean(node.InnerText));
                        Invoker.UsingIf(column.SelectSingleNode("Description"),
                            node => node != null,
                            node => sqlColumn.Description = node.InnerText);
                        Invoker.UsingIf(column.SelectSingleNode("Order"),
                            node => node != null,
                            node => sqlColumn.Order = Convert.ToInt32(node.InnerText));
                        Invoker.UsingIf(column.SelectSingleNode("TypeFullName"),
                            node => node != null,
                            node => sqlColumn.TypeFullName = node.InnerText);
                        sqlColumns.Add(sqlColumn);
                    }
                    sqlTable.Columns = sqlColumns.ToArray();
                    databaseTables.Add(ObjectResolver.TypeConvert<DatabaseTable>(sqlTable));
                }
                return databaseTables;
            }

            private IEnumerable<DatabaseTable> FromJson(string json)
            {
                JToken jToken = JToken.Parse(json);
                JArray jArray = jToken as JArray ?? jToken.First as JArray;
                JsonSerializer jsonSerializer = new JsonSerializer();
                jsonSerializer.Converters.Add(new SqlColumnJsonConverter());
                return jArray.Select(x => ObjectResolver.TypeConvert<DatabaseTable>(x.ToObject<SqlTable>(jsonSerializer))).ToList();
            }
        }
    }
}