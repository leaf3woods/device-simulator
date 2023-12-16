using System.Text.Json.Nodes;

namespace DeviceSimulator.Domain.ValueObjects.Message.Base
{
    public interface IAsJsonNode
    {
        JsonNode? AsJson();

        JsonNode? Node { get; set; }
    }
}