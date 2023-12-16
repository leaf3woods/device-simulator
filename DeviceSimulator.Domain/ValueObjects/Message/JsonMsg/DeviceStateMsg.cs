using DeviceSimulator.Domain.Entities.IotData;
using DeviceSimulator.Domain.Utilities;
using DeviceSimulator.Domain.ValueObjects.Message.Base;
using System.Text.Json.Nodes;

namespace DeviceSimulator.Domain.ValueObjects.Message.JsonMsg
{
    public class DeviceStateMsg : JsonMessage, IAsIotData<DeviceState>, IAsJsonNode
    {
        public DeviceStateMsg(string raw)
        {
            Node = JsonNode.Parse(raw) ??
                throw new ArgumentException("can't parse json node");
            IotData = ConvertToObject();
        }

        public DeviceStateMsg(DeviceState state)
        {
            MsgTime = DateTime.UtcNow;
            IotData = state;
            Node = AsJson();
            Raw = Node!.ToJsonString(Options.CustomJsonSerializerOptions);
        }

        public JsonNode? Node { get; set; }

        public DeviceState IotData { get; set; }

        public JsonNode? AsJson()
        {
            if (string.IsNullOrEmpty(Raw))
            {
                var node = new JsonObject
                {
                    { "connected", MsgTime },
                    { "mac", IotData.Mac },
                    { "version", IotData.Version },
                };
                return node;
            }
            else
            {
                return JsonNode.Parse(Raw) ?? throw new ArgumentException("unexcepted frame header");
            }
        }

        public DeviceState ConvertToObject()
        {
            if (Node == null)
            {
                throw new ArgumentNullException(nameof(Node));
            }
            return new DeviceState
            {
                Connected = Node["connected"]!.GetValue<int>(),
                Mac = Node["mac"]?.GetValue<string>(),
                Version = Node["version"]?.GetValue<string>(),
            };
        }
    }
}