using DeviceSimulator.Domain.ValueObjects.Message.Base;
using System.Text.Json.Nodes;

namespace DeviceSimulator.Domain.ValueObjects.Message.JsonMsg
{
    public class DeviceHealthMsg : JsonMessage
    {
        public DeviceHealthMsg(string raw)
        {
            Node = JsonNode.Parse(raw) ??
                throw new ArgumentException("can't parse json node");
        }

        public JsonNode? Node { get; set; }
    }
}