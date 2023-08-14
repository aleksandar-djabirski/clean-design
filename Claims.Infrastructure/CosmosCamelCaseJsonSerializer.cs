using Microsoft.Azure.Cosmos;
using Newtonsoft.Json.Serialization;
using Newtonsoft.Json;
using System.Text;

namespace Claims.Infrastructure
{
    public class CosmosCamelCaseJsonSerializer : CosmosSerializer
    {
        private static readonly JsonSerializerSettings JsonSerializerSettings = new JsonSerializerSettings()
        {
            ContractResolver = new CamelCasePropertyNamesContractResolver()
        };

        private static readonly JsonSerializer JsonSerializer = JsonSerializer.Create(JsonSerializerSettings);

        public override T FromStream<T>(Stream stream)
        {
            using (StreamReader sr = new StreamReader(stream))
            using (JsonTextReader jsonTextReader = new JsonTextReader(sr))
            {
                return JsonSerializer.Deserialize<T>(jsonTextReader);
            }
        }

        public override Stream ToStream<T>(T input)
        {
            MemoryStream streamPayload = new MemoryStream();
            using (StreamWriter streamWriter = new StreamWriter(streamPayload, encoding: Encoding.Default, bufferSize: 1024, leaveOpen: true))
            using (JsonTextWriter writer = new JsonTextWriter(streamWriter))
            {
                JsonSerializer.Serialize(writer, input);
                writer.Flush();
                streamPayload.Position = 0;
                return streamPayload;
            }
        }
    }
}
