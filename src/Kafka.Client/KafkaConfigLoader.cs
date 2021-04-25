using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Confluent.Kafka;

namespace Kafka.Client
{
    public class KafkaConfigLoader
    {
        public static async Task<ClientConfig> LoadConfig(string configPath, string certDir)
        {
            var configParams = (await File.ReadAllLinesAsync(configPath))
                .Where(line => !line.StartsWith("#"))
                .ToDictionary(
                    line => line.Substring(0, line.IndexOf('=')),
                    line => line.Substring(line.IndexOf('=') + 1));

            var clientConfig = new ClientConfig(configParams);

            if (certDir != null)
            {
                clientConfig.SslCaLocation = certDir;
            }

            return clientConfig;
        }
    }
}