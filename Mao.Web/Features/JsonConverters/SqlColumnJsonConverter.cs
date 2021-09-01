using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Mao.Web.Features.JsonConverters
{
    public class SqlColumnJsonConverter : JsonConverter
    {
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            serializer.Serialize(writer, value);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            if (objectType == typeof(bool) || objectType == typeof(bool?))
            {
                if (reader.Value == null)
                {
                    if (objectType == typeof(bool))
                    {
                        return false;
                    }
                    return null;
                }
                if (reader.Value is bool @bool)
                {
                    return @bool;
                }
                return reader.Value.ToString() == "1"
                    || reader.Value.ToString().ToLower() == "t"
                    || reader.Value.ToString().ToLower() == "true";
            }
            return reader.Value;
        }

        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(bool)
                 || objectType == typeof(bool?);
        }
    }
}