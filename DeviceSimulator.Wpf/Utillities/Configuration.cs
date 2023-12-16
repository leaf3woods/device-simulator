namespace Controller.Utillities
{
    public record class ApnSettings(string AccessPoint, string UsernameExtension, string ApPassword);

    public record class MqttSettings(string Server, string Port, string UserName, string Password);

    public record Settings(string Com, string Port, string? FlashConfigPath, MqttSettings Mqtt, string? ExcelPath, ApnSettings Apn);
}