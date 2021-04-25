using System.Collections.Generic;
using System.Threading.Tasks;
using Contract.Request;

namespace AsyncApi.Services
{
    public interface IChatService
    {
        bool Connect(ConnectRequest model);
        IEnumerable<string> GetClients();
        Task<bool> SendMessage(MessageSendRequest model);
    }
}