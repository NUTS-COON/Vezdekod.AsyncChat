using System.Threading.Tasks;
using Confluent.Kafka;
using Microsoft.Extensions.Options;

namespace Kafka.Client
{
    public class ProducerFactory<TKey, TMessage>
    {
        private readonly KafkaClientOptions _kafkaClientOptions;

        
        public ProducerFactory(IOptionsSnapshot<KafkaClientOptions> kafkaClientOptions)
        {
            _kafkaClientOptions = kafkaClientOptions.Value;
        }

        
        public async Task<IProducer<TKey, TMessage>> Create()
        {
            var config = await KafkaConfigLoader.LoadConfig(_kafkaClientOptions.ConfigPath, _kafkaClientOptions.CertificatePath);
            return new ProducerBuilder<TKey, TMessage>(config)
                .Build();
        }
    }
}