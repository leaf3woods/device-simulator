using DeviceSimulator.Domain.ValueObjects.Message.Base;

namespace DeviceSimulator.Domain.ValueObjects.Message
{
    public class IotTopic : MqttTopic
    {
        public static string[] SubTopics { get; set; } = { $"iot/+/{DevTypeName}/+/{DevUriName}/+/+" };

        public const string DevTypeName = "type";
        public const string DevUriName = "uri";
        public string? DeviceType { get; set; }
        public string? DeviceUri { get; set; }

        public static IotTopic FromIotString(string topic)
        {
            var nodes = topic.Split('/');
            var dir = nodes[1] switch
            {
                "up" => MqttDirection.Up,
                "down" => MqttDirection.Down,
                _ => throw new NotSupportedException(nodes[1])
            };
            var tag = nodes[6] switch
            {
                "state" => MqttTag.State,
                "data" => MqttTag.Data,
                "ota" => MqttTag.Ota,
                "cmd" => MqttTag.Cmd,
                "health" => MqttTag.Health,
                "calibration" => MqttTag.Calibration,
                _ => throw new NotSupportedException(nodes[6])
            };

            var res = new IotTopic
            {
                Prefix = nodes[0],
                Direction = dir,
                DeviceType = nodes[3],
                DeviceUri = nodes[5],
                Tag = tag
            };
            return res;
        }

        public override string ToString()
        {
            var tag = Tag.ToString("F").ToLower();
            var dir = Direction.ToString("F").ToLower();
            if (DeviceUri is not null)
            {
                return $"{Prefix}/{dir}/{DevTypeName}/{DeviceType}/{DevUriName}/{DeviceUri}/{tag}";
            }
            else
            {
                return $"{Prefix}/{dir}/{tag}";
            }
        }
    }
}