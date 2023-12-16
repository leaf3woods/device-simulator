namespace DeviceSimulator.Domain.ValueObjects.Message.Base
{
    public class MqttTopic
    {
        public string Prefix { get; set; } = null!;
        public MqttDirection Direction { get; set; }
        public MqttTag Tag { get; set; }

        public static MqttTopic FromString(string topic)
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

            var res = new MqttTopic
            {
                Direction = dir,
                Tag = tag
            };
            return res;
        }

        public override string ToString()
        {
            var tag = Tag!.ToString("F").ToLower();
            var dir = Direction.ToString("F").ToLower();
            return $"{Prefix}/{dir}/{tag}";
        }
    }

    public enum MqttDirection
    {
        Up,
        Down,
    }

    public enum MqttTag
    {
        State,
        Data,
        Ota,
        Cmd,
        Health,
        Calibration,
        Alarm
    }

    public enum MqttState : byte
    {
        Connected,
        Stop,
        Restart,
        Will
    }
}