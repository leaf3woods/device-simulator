using DeviceSimulator.Domain.ValueObjects.Message.Base;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using MQTTnet;
using MQTTnet.Client;
using MQTTnet.Extensions.ManagedClient;

namespace DeviceSimulator.Infrastructure.Mqtt
{
    public class ApiMqttPub : IMqttExplorer
    {
        private readonly IManagedMqttClient _mqttClient;
        private ManagedMqttClientOptions _mqttClientOptions = null!;
        private readonly ILogger<ApiMqttPub> _logger;

        public ApiMqttPub(
            ILogger<ApiMqttPub> logger,
            IConfiguration configuration)
        {
            _logger = logger;
            var mqttFactory = new MqttFactory();
            _mqttClient = mqttFactory.CreateManagedMqttClient();
            _mqttClient.DisconnectedAsync += async _ =>
                await Task.Run(() => { _logger.LogError("mqtt disconnected"); });
        }

        public async Task StartAsync(string server, int port, string username, string password)
        {
            _mqttClientOptions = new ManagedMqttClientOptionsBuilder()
                .WithAutoReconnectDelay(TimeSpan.FromSeconds(10))
                .WithClientOptions(new MqttClientOptionsBuilder()
                    .WithTcpServer(server, port)
                    .WithCredentials(username, password)
                    .WithKeepAlivePeriod(TimeSpan.FromSeconds(10))
                    .WithWillQualityOfServiceLevel(MQTTnet.Protocol.MqttQualityOfServiceLevel.ExactlyOnce)
                    .WithWillTopic(TopicBuilder.CreateBuilder()
                        .WithPrefix("system")
                        .WithDirection(MqttDirection.Up)
                        .WithTag(MqttTag.State)
                        .Build())
                    .WithWillPayload(new byte[] { (byte)MqttState.Will })
                    .WithCleanSession()
                    .Build())
                .Build();
            await _mqttClient.StartAsync(_mqttClientOptions);
        }

        public async Task RestartAsync(string server, int port, string username, string password)
        {
            _mqttClientOptions = new ManagedMqttClientOptionsBuilder()
                .WithAutoReconnectDelay(TimeSpan.FromSeconds(10))
                .WithClientOptions(new MqttClientOptionsBuilder()
                    .WithTcpServer(server, port)
                    .WithCredentials(username, password)
                    .WithKeepAlivePeriod(TimeSpan.FromSeconds(10))
                    .WithWillQualityOfServiceLevel(MQTTnet.Protocol.MqttQualityOfServiceLevel.ExactlyOnce)
                    .WithWillTopic(TopicBuilder.CreateBuilder()
                        .WithPrefix("system")
                        .WithDirection(MqttDirection.Up)
                        .WithTag(MqttTag.State)
                        .Build())
                    .WithWillPayload(new byte[] { (byte)MqttState.Will })
                    .WithCleanSession()
                    .Build())
                .Build();
            var message = new MqttApplicationMessageBuilder()
            .WithTopic(TopicBuilder.CreateBuilder()
                        .WithPrefix("system")
                        .WithDirection(MqttDirection.Up)
                        .WithTag(MqttTag.State)
                        .Build())
            .WithPayload(new byte[] { (byte)MqttState.Restart })
            .WithQualityOfServiceLevel(MQTTnet.Protocol.MqttQualityOfServiceLevel.AtLeastOnce)
            .Build();
            await _mqttClient.EnqueueAsync(message);
            await _mqttClient.StopAsync();
            await _mqttClient.StartAsync(_mqttClientOptions);
        }

        public async Task StopAsync()
        {
            var message = new MqttApplicationMessageBuilder()
            .WithTopic(TopicBuilder.CreateBuilder()
                        .WithPrefix("system")
                        .WithDirection(MqttDirection.Up)
                        .WithTag(MqttTag.State)
                        .Build())
            .WithPayload(new byte[] { (byte)MqttState.Stop })
            .WithQualityOfServiceLevel(MQTTnet.Protocol.MqttQualityOfServiceLevel.AtLeastOnce)
            .Build();
            await _mqttClient.EnqueueAsync(message);
            await _mqttClient.StopAsync();
        }

        public async Task PublishAsync(string Topic, byte[] payload)
        {
            if (!_mqttClient.IsConnected)
                await _mqttClient.StartAsync(_mqttClientOptions);
            var message = new MqttApplicationMessageBuilder()
                .WithTopic(Topic)
                .WithPayload(payload)
                .WithQualityOfServiceLevel(MQTTnet.Protocol.MqttQualityOfServiceLevel.AtLeastOnce)
                .Build();
            await _mqttClient.EnqueueAsync(message);
        }
    }
}