using System.Threading.Tasks;
using AsyncApi.Models;
using Confluent.Kafka;
using Contract;
using Kafka.Client;
using Newtonsoft.Json;

namespace AsyncApi.Services
{
    public class KafkaService : IKafkaService
    {
        private IProducer<string, string> _producer;
        private readonly ProducerFactory<string, string> _producerFactory;


        public KafkaService(ProducerFactory<string, string> producerFactory)
        {
            _producerFactory = producerFactory;
        }
        
        
        public async Task<bool> Send(KafkaMessage message)
        {
            if (_producer == null)
            {
                _producer = await _producerFactory.Create();
            }
            
            var chatMessage = new ChatMessage
            {
                Sender = message.Sender,
                Text = message.Text
            };
            var data = JsonConvert.SerializeObject(chatMessage);
            
            var result = await _producer.ProduceAsync(message.Topic, new Message<string, string> { Key = "key", Value = data });

            return result.Status == PersistenceStatus.Persisted || 
                   result.Status == PersistenceStatus.PossiblyPersisted;
        }
    }
}