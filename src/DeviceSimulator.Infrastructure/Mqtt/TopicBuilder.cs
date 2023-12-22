using DeviceSimulator.Domain.ValueObjects.Message;
using DeviceSimulator.Domain.ValueObjects.Message.Base;

namespace DeviceSimulator.Infrastructure.Mqtt
{
    public class TopicBuilder
    {
        protected string? _prefix ;
        protected MqttDirection _direction;
        protected MqttTag _tag;

        protected TopicBuilder()
        { }

        public virtual TopicBuilder WithPrefix(string prefix)
        {
            _prefix = prefix;
            return this;
        }

        public virtual TopicBuilder WithDirection(MqttDirection direction)
        {
            _direction = direction;
            return this;
        }

        public virtual TopicBuilder WithTag(MqttTag tag)
        {
            _tag = tag;
            return this;
        }

        public virtual string Build() =>
            new MqttTopic { Direction = _direction, Prefix = _prefix ?? throw new ArgumentNullException(), Tag = _tag }.ToString();

        public static TopicBuilder CreateBuilder() => new();
    }

    public class IotTopicBuilder : TopicBuilder
    {
        private string _deviceType = null!;
        private string _deviceUri = null!;

        private IotTopicBuilder()
        { }

        public override IotTopicBuilder WithPrefix(string prefix)
        {
            _prefix = prefix;
            return this;
        }

        public override IotTopicBuilder WithDirection(MqttDirection direction)
        {
            _direction = direction;
            return this;
        }

        public override IotTopicBuilder WithTag(MqttTag tag)
        {
            _tag = tag;
            return this;
        }

        public IotTopicBuilder WithDeviceType(string deviceType)
        {
            _deviceType = deviceType;
            return this;
        }

        public IotTopicBuilder WithUri(string uri)
        {
            _deviceUri = uri;
            return this;
        }

        public override string Build() =>
            new IotTopic
            {
                Direction = _direction,
                Prefix = _prefix ?? "iot",
                Tag = _tag,
                DeviceType = _deviceType,
                DeviceUri = _deviceUri
            }.ToString();

        public static new IotTopicBuilder CreateBuilder() => new();
    }

    public class UserTopicBuilder : TopicBuilder
    {
        private string _patientCode = null!;

        private UserTopicBuilder()
        { }

        public override UserTopicBuilder WithPrefix(string prefix)
        {
            _prefix = prefix;
            return this;
        }

        public override UserTopicBuilder WithDirection(MqttDirection direction)
        {
            _direction = direction;
            return this;
        }

        public override UserTopicBuilder WithTag(MqttTag tag)
        {
            _tag = tag;
            return this;
        }

        public UserTopicBuilder WithPatientCode(string patientCode)
        {
            _patientCode = patientCode;
            return this;
        }

        public override string Build() =>
            new UserTopic
            {
                Direction = _direction,
                Prefix = _prefix ?? "user",
                Tag = _tag,
                PatientCode = _patientCode
            }.ToString();

        public static UserTopicBuilder CreateUserBuilder() => new();
    }
}