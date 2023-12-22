using System.Text.Json.Nodes;

namespace DeviceSimulator.Domain.ValueObjects.Message.Base
{
    public class JsonMessage : IotMessage
    {
        public JsonMessage(string raw)
        { 
            Raw = raw;
        }

        public JsonMessage()
        {
        }

        public string? Raw { get; set; }
        JsonNode? Node { get; set; }
    }
}