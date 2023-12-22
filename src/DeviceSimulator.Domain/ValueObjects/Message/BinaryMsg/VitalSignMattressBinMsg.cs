using DeviceSimulator.Domain.Entities.IotData;
using DeviceSimulator.Domain.Utilities;
using DeviceSimulator.Domain.ValueObjects.Message.Base;
using System.Text.Json.Nodes;

namespace DeviceSimulator.Domain.ValueObjects.Message
{
    public class VitalSignMattressBinMsg : BinaryMessage, IAsJsonNode, IAsIotData<VitalSign>
    {

        public VitalSignMattressBinMsg(byte[] raw)
        {
            Verify(raw);
            FrameType = raw[4];
            FrameData = raw[5..(raw.Length - 4)];
            MsgTime = DateTime.UnixEpoch.AddMilliseconds(BitConverter.ToInt64(FrameData.AsSpan()[..8]));
            IotData = ConvertToObject();
            Node = AsJson();
        }

        public VitalSignMattressBinMsg(VitalSign vitalSign)
        {
            MsgTime = DateTime.UtcNow;
            IotData = vitalSign;
            FrameType = 0x01;
            FrameData = AsFrameData();
        }

        public JsonNode? Node { get; set; }

        public VitalSign IotData { get; set; } = null!;

        public JsonNode? AsJson()
        {
            var node = new JsonObject();
            switch (FrameType)
            {
                case 0x1:
                    {
                        node.Add("time", BitConverter.ToInt64(FrameData.AsSpan()[..8]));
                        node.Add("heart", FrameData.ElementAtOrDefault(8));
                        node.Add("breath", FrameData.ElementAtOrDefault(9));
                        node.Add("move", FrameData.ElementAtOrDefault(10));
                        node.Add("state", FrameData.ElementAtOrDefault(11));

                        if (FrameData.Length <= 4) return node;
                        // 开发阶段的光功率数据
                        var opticalPowerPayload = FrameData[4..];
                        var opticalPowerValues = Enumerable.Range(0, opticalPowerPayload.Length / 4).Select(index =>
                            JsonValue.Create<float>(BitConverter.ToSingle(opticalPowerPayload.AsSpan()[(index * 4)..(index * 4 + 4)]))).ToArray();
                        node.Add("opticalPowerValues", new JsonArray(opticalPowerValues));
                        node.Add("opticalPowerCount", opticalPowerValues.Length);
                        return node;
                    }
                case 0x2:
                    {
                        node.Add("score", FrameData.ElementAtOrDefault(0));
                        node.Add("efficiency", FrameData.ElementAtOrDefault(1));
                        node.Add("pkgCount", FrameData.ElementAtOrDefault(2));
                        node.Add("pkgIndex", FrameData.ElementAtOrDefault(3));
                        //Payload.Add("status", PayloadBuffer.ElementAtOrDefault(4));
                        if (FrameData.Length <= 4) return node;
                        var statusValues = FrameData[4..].Select(x => JsonValue.Create<int>(x)).ToArray();
                        node.Add("status", new JsonArray(statusValues));
                        return node;
                    }
                default:
                    return null;
            }
        }

        public override byte[] AsFrame()
        {
            var payloadCount = 5 + FrameData.Length;
            var headers = new[] { (byte)payloadCount, (byte)(payloadCount >> 8), FrameType };
            var dataBytes = FrameHeader.Concat(headers).Concat(FrameData);
            return [..dataBytes.AppendCrc32()];
        }

        private byte[] AsFrameData()
        {
            long stamp = (long)(MsgTime - DateTime.UnixEpoch).TotalMilliseconds;
            var buff = BitConverter.GetBytes(stamp);
            var payload = new byte[4]
            {
                (byte)IotData.Heart,
                (byte)IotData.Breath,
                (byte)IotData.Move,
                (byte)IotData.State
            };
            var result = buff.Concat(payload);
            return result.ToArray();
        }

        public override bool Verify(byte[] raw)
        {
            if (!raw[..2].SequenceEqual(DefaultFrameHeader))
            {
                return false;
            }

            if (!raw.CheckCrc32())
            {
                throw new ArgumentException("crc check failed");
            }
            return true;
        }

        public VitalSign ConvertToObject()
        {
            return new VitalSign
            {
                Heart = FrameData.ElementAtOrDefault(8),
                Breath = FrameData.ElementAtOrDefault(9),
                Move = FrameData.ElementAtOrDefault(10),
                State = FrameData.ElementAtOrDefault(11)
            };
        }
    }

    public enum MattressState
    {
        Leave = 3, //离床
        On = 4, //在床
        Error = 5, //故障
        Offline = 6, //离线
        Occupy = 9,//传感器负荷
        WeakSignal = 10 //信号弱
    }
}