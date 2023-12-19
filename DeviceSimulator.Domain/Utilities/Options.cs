using System.Text.Json;
using System.Text.Json.Serialization;

namespace DeviceSimulator.Domain.Utilities
{
    public static class Options
    {
        public static JsonSerializerOptions CustomJsonSerializerOptions { get; set; } = new JsonSerializerOptions
        {
            AllowTrailingCommas = true,
            PropertyNameCaseInsensitive = false,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            ReferenceHandler = ReferenceHandler.IgnoreCycles,
            WriteIndented = true,
        };
    }
}