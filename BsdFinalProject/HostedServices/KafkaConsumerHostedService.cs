using Confluent.Kafka;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using System.Text.Json;

namespace BsdFinalProject.HostedServices
{
    public class KafkaConsumerHostedService : BackgroundService
    {
        private readonly ILogger<KafkaConsumerHostedService> _logger;
        private readonly IConfiguration _config;
        private IConsumer<string, string>? _consumer;
        private string _topic = "transactions";

        public KafkaConsumerHostedService(ILogger<KafkaConsumerHostedService> logger, IConfiguration config)
        {
            _logger = logger;
            _config = config;
            _topic = config["Kafka:Topic"] ?? _topic;
        }

        public override Task StartAsync(CancellationToken cancellationToken)
        {
            var bootstrap = _config["Kafka:BootstrapServers"] ?? "localhost:9092";
            var consumerConfig = new ConsumerConfig
            {
                BootstrapServers = bootstrap,
                GroupId = "bsd_consumer_group",
                AutoOffsetReset = AutoOffsetReset.Earliest,
                EnableAutoCommit = true
            };

            _consumer = new ConsumerBuilder<string, string>(consumerConfig).Build();
            _consumer.Subscribe(_topic);

            _logger.LogInformation("Kafka consumer subscribed to topic {Topic}", _topic);
            return base.StartAsync(cancellationToken);
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            return Task.Run(() =>
            {
                try
                {
                    while (!stoppingToken.IsCancellationRequested && _consumer != null)
                    {
                        try
                        {
                            var cr = _consumer.Consume(stoppingToken);
                            if (cr == null) continue;

                            _logger.LogInformation("Consumed message from topic {Topic} partition {Partition} offset {Offset}: {Value}", cr.Topic, cr.Partition, cr.Offset, cr.Message.Value);
                        }
                        catch (ConsumeException ex)
                        {
                            _logger.LogError(ex, "Consume error");
                        }
                    }
                }
                catch (OperationCanceledException) { }
            }, stoppingToken);
        }

        public override Task StopAsync(CancellationToken cancellationToken)
        {
            try
            {
                _consumer?.Close();
                _consumer?.Dispose();
            }
            catch { }
            return base.StopAsync(cancellationToken);
        }
    }
}
