using DeviceSimulator.Domain.Utilities;

namespace DeviceSimulator.Domain.ValueObjects.Message.Base
{
    public abstract class BinaryMessage : IotMessage
    {
        public static byte[] DefaultFrameHeader { get => new byte[] { 0xA5, 0x5A }; }
        public byte[] FrameHeader { get; set; } = DefaultFrameHeader;
        public int FrameLength { get => 5 + FrameData.Length; }
        public byte FrameType { get; set; }
        public byte[] FrameData { get; set; } = Array.Empty<byte>();

        public virtual byte[] AsFrame()
        {
            var payloadCount = 5 + FrameData.Length;
            var headers = new[] { (byte)payloadCount, (byte)(payloadCount >> 8), FrameType };
            var dataBytes = FrameHeader.Concat(headers).Concat(FrameData);
            return dataBytes.AppendCrc32().ToArray();
        }

        public virtual bool Verify(byte[] raw)
        {
            if (!raw.CheckCrc32())
            {
                throw new ArgumentException("crc check failed");
            }
            return true;
        }
    }
}