using Confluent.Kafka;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace BsdFinalProject.Services
{
    public interface IKafkaProducer
    {
        Task ProduceAsync<T>(T message, string? topic = null);
    }

    public class KafkaProducer : IKafkaProducer, IDisposable
    {
        private readonly IProducer<string, string> _producer;
        private readonly string _topic;
        private readonly ILogger<KafkaProducer> _logger;

        public KafkaProducer(IConfiguration config, ILogger<KafkaProducer> logger)
        {
            _logger = logger;
            var bootstrap = config["Kafka:BootstrapServers"] ?? "localhost:9092";
            _topic = config["Kafka:Topic"] ?? "transactions";

            var producerConfig = new ProducerConfig
            {
                BootstrapServers = bootstrap,
                Acks = Acks.All
            };

            _producer = new ProducerBuilder<string, string>(producerConfig).Build();
        }

        public async Task ProduceAsync<T>(T message, string? topic = null)
        {
            var t = topic ?? _topic;
            var payload = JsonSerializer.Serialize(message);
            try
            {
                var dr = await _producer.ProduceAsync(t, new Message<string, string> { Key = null, Value = payload });
                _logger.LogInformation("Produced message to {Topic} partition {Partition} @ offset {Offset}", dr.Topic, dr.Partition, dr.Offset);
            }
            catch (ProduceException<string, string> ex)
            {
                _logger.LogError(ex, "Error producing message to Kafka");
            }
        }

        public void Dispose()
        {
            try
            {
                _producer?.Flush(TimeSpan.FromSeconds(5));
                _producer?.Dispose();
            }
            catch { }
        }
    }
}
