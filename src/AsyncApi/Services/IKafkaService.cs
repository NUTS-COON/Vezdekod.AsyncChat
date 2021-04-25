using System.Threading.Tasks;
using AsyncApi.Models;

namespace AsyncApi.Services
{
    public interface IKafkaService
    {
        Task<bool> Send(KafkaMessage message);
    }
}