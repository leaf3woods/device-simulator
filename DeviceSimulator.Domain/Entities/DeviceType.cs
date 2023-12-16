
using DeviceSimulator.Domain.Entities.Base;

namespace DeviceSimulator.Domain.Entities
{
    public class DeviceType : IncrementEntity
    {
        public string Code { get; set; } = null!;
        public string Name { get; set; } = null!;

        #region navigation

        public virtual ICollection<Device> Devices { get; set; } = new List<Device>();

        #endregion navigation

        public static readonly DeviceType VitalSignMonitoringMattress = new()
        {
            Id = 1,
            Code = "10004",
            Name = "Contactless VitalSign Monitoring Mattress"
        };

        public static readonly DeviceType[] Seeds = [VitalSignMonitoringMattress];
    }
}
