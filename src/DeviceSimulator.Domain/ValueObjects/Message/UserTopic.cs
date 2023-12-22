using DeviceSimulator.Domain.ValueObjects.Message.Base;

namespace DeviceSimulator.Domain.ValueObjects.Message
{
    public class UserTopic : MqttTopic
    {
        public string PatientCode { get; set; } = null!;
        public const string PatientCodeName = "patient";

        public static UserTopic FromUserString(string topic)
        {
            var nodes = topic.Split('/');
            var dir = nodes[1] switch
            {
                "up" => MqttDirection.Up,
                "down" => MqttDirection.Down,
                _ => throw new NotSupportedException()
            };
            var tag = nodes[^1] switch
            {
                "state" => MqttTag.State,
                "data" => MqttTag.Data,
                "ota" => MqttTag.Ota,
                "cmd" => MqttTag.Cmd,
                "health" => MqttTag.Health,
                "calibration" => MqttTag.Calibration,
                _ => throw new NotSupportedException()
            };

            var res = new UserTopic
            {
                Direction = dir,
                PatientCode = nodes[3],
                Tag = tag
            };
            return res;
        }

        public override string ToString()
        {
            var tag = Tag.ToString("F").ToLower();
            var dir = Direction.ToString("F").ToLower();
            if (!string.IsNullOrEmpty(PatientCode))
            {
                return $"{Prefix}/{dir}/{PatientCodeName}/{PatientCode}/{tag}";
            }
            else
            {
                return $"{Prefix}/{dir}/{tag}";
            }
        }
    }
}