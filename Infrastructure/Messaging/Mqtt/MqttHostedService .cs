using Application.Abstractions.CasheStorage.Mqtt;
using Application.Abstractions.Messaging.Mqtt;
using Application.Contracts.Messaging.Mqtt;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MQTTnet.Client;
using MQTTnet.Extensions.ManagedClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace Infrastructure.Messaging.Mqtt
{
    public class MqttHostedService : BackgroundService
    {
        private readonly ILogger<MqttHostedService> _logger;
        private readonly IManagedMqttClient _client;
        private readonly MqttOptions _options;
        private readonly IMqttTopicBuilder _topicBuilder;
        private readonly IMqttBus _bus;

        public MqttHostedService(
            ILogger<MqttHostedService> logger, 
            IManagedMqttClient client, 
            IOptions<MqttOptions> options,
            IMqttTopicBuilder topicBuilder,
            IMqttBus bus)
        {
            _logger = logger;
            _client = client;
            _options = options.Value;
            _topicBuilder = topicBuilder;
            _bus = bus;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            // 3) Build options (connect/reconnect managed client)
            var clientOptions = new MqttClientOptionsBuilder()
                .WithClientId(_options.ClientId)
                .WithTcpServer(_options.Host, _options.Port)
                .WithCleanSession(_options.CleanSession)
                .WithKeepAlivePeriod(TimeSpan.FromSeconds(_options.KeepAliveSeconds));

            if (!string.IsNullOrWhiteSpace(_options.Username))
                clientOptions = clientOptions.WithCredentials(_options.Username, _options.Password);

            var managedOptions = new ManagedMqttClientOptionsBuilder()
                .WithClientOptions(clientOptions.Build())
                .WithAutoReconnectDelay(TimeSpan.FromSeconds(_options.AutoReconnectDelaySeconds))
                .Build();

            // 4) Start the managed client (will auto-reconnect)
            _logger.LogInformation("MqttHostedService: ExecuteAsync: Starting MQTT managed client: {Host}:{Port}, ClientId={ClientId}",
                _options.Host, _options.Port, _options.ClientId);

            await _client.StartAsync(managedOptions);

            // 5) Subscribe to topics you want to RECEIVE from devices
            await _bus.SubscribeAsync(_topicBuilder.ControlUnitState());
            await _bus.SubscribeAsync(_topicBuilder.ControlUnitTelemetry());

            // 6) Keep service alive until shutdown
            // Managed client runs in background; we only wait here.
            while (!stoppingToken.IsCancellationRequested)
            {
                await Task.Delay(TimeSpan.FromSeconds(10), stoppingToken);
            }
        }
    }
}
