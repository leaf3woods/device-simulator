
using DeviceSimulator.Domain.Entities.Base;

namespace DeviceSimulator.Domain.Entities
{
    public class Device : IncrementEntity
    {
        public string Uri { get; set; } = null!;
        public string? Name { get; set; }

        #region navigation

        public string DeviceTypeCode { get; set; } = null!;
        public DeviceType DeviceType { get; set; } = null!;

        #endregion navigation

    }
}
