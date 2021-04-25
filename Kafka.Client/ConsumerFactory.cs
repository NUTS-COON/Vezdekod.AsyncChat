using System.Threading.Tasks;
using Confluent.Kafka;
using Microsoft.Extensions.Options;

namespace Kafka.Client
{
    public class ConsumerFactory<TKey, TMessage>
    {
        private readonly KafkaClientOptions _kafkaClientOptions;

        
        public ConsumerFactory(IOptionsSnapshot<KafkaClientOptions> kafkaClientOptions)
        {
            _kafkaClientOptions = kafkaClientOptions.Value;
        }
        
        
        public async Task<IConsumer<TKey, TMessage>> Create()
        {
            var config = await KafkaConfigLoader.LoadConfig(_kafkaClientOptions.ConfigPath, _kafkaClientOptions.CertificatePath);
            var consumerConfig = new ConsumerConfig(config)
            {
                GroupId = "chat-client",
                AutoOffsetReset = AutoOffsetReset.Earliest, 
                EnableAutoCommit = false
            };

            return new ConsumerBuilder<TKey, TMessage>(consumerConfig).Build();
        }
    }
}