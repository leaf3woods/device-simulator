using DeviceSimulator.Domain.Entities.IotData;
using DeviceSimulator.Domain.Utilities;
using DeviceSimulator.Domain.ValueObjects.Message.Base;
using System.Text.Json.Nodes;

namespace DeviceSimulator.Domain.ValueObjects.Message.JsonMsg
{
    public class VitalSignMattressJsonMsg : JsonMessage, IAsJsonNode, IAsIotData<VitalSign>
    {
        public VitalSignMattressJsonMsg(string raw) : base(raw)
        {
            Node = AsJson();
            IotData = ConvertToObject();
        }
        public VitalSignMattressJsonMsg(VitalSign vitalSign) : base()
        {
            MsgTime = DateTime.UtcNow;
            IotData = vitalSign;
            Node = AsJson();
            Raw = Node!.ToJsonString(Options.CustomJsonSerializerOptions);
        }

        public JsonNode? Node { get; set; }

        public VitalSign IotData {  get; set; }

        public JsonNode? AsJson()
        {
            if (string.IsNullOrEmpty(Raw))
            {
                var node = new JsonObject
                {
                    { "time", (ulong)(MsgTime-DateTime.UnixEpoch).TotalMilliseconds },
                    { "heart", IotData.Heart },
                    { "breath", IotData.Breath },
                    { "move", IotData.Move },
                    { "state", IotData.State }
                };
                return node;
            }
            else
            {
                return JsonNode.Parse(Raw) ?? throw new ArgumentException("unexcepted frame header");
            }
        }

        public VitalSign ConvertToObject()
        {
            if(Node == null)
            {
                throw new ArgumentNullException(nameof(Node));
            }
            return new VitalSign
            {
                Heart = Node["time"]!.GetValue<float>(),
                Breath = Node["heart"]!.GetValue<float>(),
                Move = Node["move"]!.GetValue<float>(),
                State = Node["state"]!.GetValue<float>()
            };
        }
    }
}