using Confluent.Kafka;

namespace Events.Services;

public class KafkaConsumer : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var kafkaAddress = Environment.GetEnvironmentVariable("KAFKA_BROKERS") ?? "localhost:9092";
        var config = new ConsumerConfig
        {
            BootstrapServers = kafkaAddress,
            GroupId = "events-service",
            AutoOffsetReset = AutoOffsetReset.Earliest
        };

        using var consumer = new ConsumerBuilder<Ignore, string>(config).Build();
        consumer.Subscribe(new[] { "user-events", "payment-events", "movie-events" });

        try
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                var result = await Task.Run(() => consumer.Consume(stoppingToken), stoppingToken);
                Console.WriteLine($"Consumed message: {result.Message.Value}");
            }
        }
        catch (OperationCanceledException)
        {
            // Graceful shutdown
            Console.WriteLine("Consumer stopped.");
        }
        finally
        {
            consumer.Close();
        }
    }
}
