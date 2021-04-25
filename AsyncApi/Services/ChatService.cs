using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AsyncApi.Models;
using Contract.Request;

namespace AsyncApi.Services
{
    public class ChatService : IChatService
    {
        private readonly ClientProvider _clientProvider;
        private readonly IKafkaService _kafkaService;

        public ChatService(IKafkaService kafkaService, ClientProvider clientProvider)
        {
            _kafkaService = kafkaService;
            _clientProvider = clientProvider;
        }

        public bool Connect(ConnectRequest model)
        {
            return _clientProvider.TryAdd(model.Name, model.Id);
        }

        public IEnumerable<string> GetClients()
        {
            return _clientProvider.GetClients();
        }

        public async Task<bool> SendMessage(MessageSendRequest model)
        {
            return await _kafkaService.Send(new KafkaMessage
            {
                Topic = model.To,
                Text = model.Text,
                Sender = model.Sender
            });
        }
    }
}