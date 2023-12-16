using DeviceSimulator.Domain.ValueObjects.Message.Base;

namespace DeviceSimulator.Domain.ValueObjects.Message.BinaryMsg
{
    public class DeviceCalibrationMsg : BinaryMessage
    {
        public DeviceCalibrationMsg()
        {
            var timeOffset = DateTimeOffset.Now.ToUnixTimeMilliseconds();
            FrameHeader = DefaultFrameHeader;
            FrameType = 0x20;
            FrameData = BitConverter.GetBytes(timeOffset);
        }
    }
}