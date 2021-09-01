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
    /// <summary>
    /// 把 SqlColumn 的序列化結果轉換成 DatabaseTableColumn
    /// </summary>
    public class ConvertFromSqlColumnsSerialized
    {
        public class Request : IRequest<Response>
        {
            public string JsonOrXml { get; set; }
        }

        public class Response
        {
            public bool IsSuccessed { get; set; }
            public string Message { get; set; }
            public IEnumerable<DatabaseTableColumn> Columns { get; set; }
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
                            response.Columns = FromXml(jsonOrXml);
                            response.IsSuccessed = true;
                        }
                        if ((jsonOrXml.StartsWith("{") && jsonOrXml.EndsWith("}"))
                            || (jsonOrXml.StartsWith("[") && jsonOrXml.EndsWith("]")))
                        {
                            response.Columns = FromJson(jsonOrXml);
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

            private IEnumerable<DatabaseTableColumn> FromXml(string xml)
            {
                List<DatabaseTableColumn> databaseTableColumns = new List<DatabaseTableColumn>();
                XmlDocument doc = new XmlDocument();
                doc.LoadXml(xml);
                var columns = doc.DocumentElement.SelectNodes("column");
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
                    databaseTableColumns.Add(ObjectResolver.TypeConvert<DatabaseTableColumn>(sqlColumn));
                }
                return databaseTableColumns;
            }

            private IEnumerable<DatabaseTableColumn> FromJson(string json)
            {
                JToken jToken = JToken.Parse(json);
                JArray jArray = jToken as JArray ?? jToken.First as JArray;
                JsonSerializer jsonSerializer = new JsonSerializer();
                jsonSerializer.Converters.Add(new SqlColumnJsonConverter());
                return jArray.Select(x => ObjectResolver.TypeConvert<DatabaseTableColumn>(x.ToObject<SqlColumn>(jsonSerializer))).ToList();
            }
        }
    }
}