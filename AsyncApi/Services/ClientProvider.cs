using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace AsyncApi.Services
{
    public class ClientProvider
    {
        private readonly ConcurrentDictionary<string, string> _clients = new ConcurrentDictionary<string, string>();

        public bool TryAdd(string name, string id)
        {
            return _clients.TryAdd(name, id);
        }

        public IEnumerable<string> GetClients()
        {
            return _clients.Keys.ToList();
        }
    }
}