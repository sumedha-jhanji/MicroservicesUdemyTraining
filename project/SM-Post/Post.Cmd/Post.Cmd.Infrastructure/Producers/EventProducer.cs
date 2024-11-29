using Confluent.Kafka;
using CQRS.Core.Events;
using CQRS.Core.Producers;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Post.Cmd.Infrastructure.Producers
{
    public class EventProducer : IEventProducer
    {
        private readonly ProducerConfig _config;

        public EventProducer(IOptions<ProducerConfig> config) 
        {
            _config = config.Value;

        }
        public async Task ProduceAsync<T>(string topic, T @event) where T : BaseEvent
        {
            //instantiated new kafka producer using producer builder
            using var producer = new ProducerBuilder<string, string>(_config)   
                    .SetKeySerializer(Serializers.Utf8)
                    .SetValueSerializer(Serializers.Utf8)
                    .Build(); // return constructed kafka producer


            //created a new event message
            var eventMessage = new Message<string, string>
            {
                Key = Guid.NewGuid().ToString(),
                Value = JsonSerializer.Serialize(@event, @event.GetType()),// serialize the event object to json
            };

            //produce to kafka
            var deliveryResult = await producer.ProduceAsync(topic, eventMessage);

            if (deliveryResult.Status == PersistenceStatus.NotPersisted)
            {   
                throw new Exception($"Could not produce {@event.GetType().Name} message to topic - {topic} due tothe following reason: {deliveryResult.Message}");
            }
        }
    }
}
