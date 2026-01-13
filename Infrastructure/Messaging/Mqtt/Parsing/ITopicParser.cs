using Application.Contracts.Messaging.Mqtt;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Messaging.Mqtt.Parsing
{
    public interface ITopicParser
    {
        TopicIds Parse(string topic);
    }
}
