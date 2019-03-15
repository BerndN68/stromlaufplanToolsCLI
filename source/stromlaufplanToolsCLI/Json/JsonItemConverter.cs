using System;
using System.Windows.Media.TextFormatting;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using stromlaufplanToolsCLI.Stromlaufplan.Models;

namespace stromlaufplanToolsCLI.Json
{
    public class JsonItemConverter : Newtonsoft.Json.Converters.CustomCreationConverter<TreeNodeDataBase>
    {
        public override TreeNodeDataBase Create(Type objectType)
        {
            throw new NotImplementedException();
        }

        public TreeNodeDataBase Create(Type objectType, JObject jObject)
        {
            var type = (string)jObject.Property("type");

            if (type.EndsWith("out"))
            {
                return new TreeNodeDataOut();
            }

            return new TreeNodeDataNotSupported(jObject.ToString());

        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            // Load JObject from stream
            JObject jObject = JObject.Load(reader);

            // Create target object based on JObject
            var target = Create(objectType, jObject);

            // Populate the object properties
            serializer.Populate(jObject.CreateReader(), target);

            return target;
        }
    }

}