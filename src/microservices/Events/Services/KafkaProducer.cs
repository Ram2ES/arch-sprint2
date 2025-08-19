using Confluent.Kafka;

namespace Events.Services;

public interface IKafkaProducer
{
    Task ProduceAsync(string topic, string message);
}

public class KafkaProducer : IKafkaProducer
{
    private readonly IProducer<Null, string> _producer;

    public KafkaProducer()
    {
        var kafkaAddress = Environment.GetEnvironmentVariable("KAFKA_BROKERS") ?? "localhost:9092";
        var config = new ProducerConfig { BootstrapServers = kafkaAddress };
        _producer = new ProducerBuilder<Null, string>(config).Build();
    }

    public async Task ProduceAsync(string topic, string message)
    {
        await _producer.ProduceAsync(topic, new Message<Null, string> { Value = message });
    }
}
