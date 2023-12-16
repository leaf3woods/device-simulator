

namespace DeviceSimulator.Domain.ValueObjects.Message.Base
{
    public interface IAsIotData<TIotData>
        where TIotData : class
    {
        TIotData IotData { get; }
        TIotData ConvertToObject();
    }
}
