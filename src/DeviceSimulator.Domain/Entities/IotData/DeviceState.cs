namespace DeviceSimulator.Domain.Entities.IotData
{
    public class DeviceState
    {
        public int Connected { get; set; }
        public string? Mac { get; set; }
        public string? Version { get; set; }
    }
}
