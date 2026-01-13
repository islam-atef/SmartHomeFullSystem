namespace Infrastructure.Messaging.Mqtt
{
    public sealed class MqttOptions
    {
        public string Host { get; set; } = "localhost";
        public int Port { get; set; } = 1883;

        public string ClientId { get; set; } = "SmartHome.Api";
        public bool CleanSession { get; set; } = true;

        public int KeepAliveSeconds { get; set; } = 30;
        public int AutoReconnectDelaySeconds { get; set; } = 5;

        public string? Username { get; set; }
        public string? Password { get; set; }
    }
}