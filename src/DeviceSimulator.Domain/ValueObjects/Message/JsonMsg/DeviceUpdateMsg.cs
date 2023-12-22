namespace DeviceSimulator.Domain.ValueObjects.Message.JsonMsg
{
    public class DeviceUpdateMsg
    {
        public string Version { get; set; } = null!;
        public string Type { get; set; } = null!;
        public string Url { get; set; } = null!;

        public IDictionary<string, DeviceModuleMsg> Mods { get; set; } =
            new Dictionary<string, DeviceModuleMsg>();
    }

    public class DeviceModuleMsg
    {
        public string Version { get; set; } = null!;
        public string Path { get; set; } = null!;
        public string? Checksum { get; set; }
    }
}