using System.Collections.Generic;

namespace Contract.Response
{
    public class ClientsResponse
    {
        public IEnumerable<string> Users { get; set; }
    }
}