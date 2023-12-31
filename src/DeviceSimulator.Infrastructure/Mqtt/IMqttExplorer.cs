﻿namespace DeviceSimulator.Infrastructure.Mqtt
{
    public interface IMqttExplorer
    {
        Task StartAsync(string server, int port, string username, string password);

        Task StopAsync();

        Task PublishAsync(string Topic, byte[] payload);

        Task RestartAsync(string server, int port, string username, string password);
    }
}