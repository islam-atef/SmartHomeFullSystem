using Microsoft.Extensions.Hosting;
using MQTTnet.Extensions.ManagedClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Messaging.Mqtt
{
    public class MqttHostedService : BackgroundService
    {
        private readonly IManagedMqttClient _client;
        public MqttHostedService(IManagedMqttClient client)
        {
            _client = client;
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            throw new NotImplementedException();
        }
    }
}
