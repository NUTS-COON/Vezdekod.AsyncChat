using System;
using System.IO;
using System.Threading.Tasks;
using Kafka.Client;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace ChatClient
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var host = new HostBuilder()
                .UseContentRoot(Path.GetDirectoryName(typeof(Program).Assembly.Location))
                .ConfigureAppConfiguration((hostingContext, config) =>
                {
                    config.AddJsonFile($"appsettings.json", optional: false, reloadOnChange: true);
                })
                .ConfigureServices((hostContext, services) =>
                {
                    services.AddScoped<ChatService>();
                    services.AddScoped<AsyncApiService>();
                    services.AddScoped(typeof(ConsumerFactory<,>));
                    services.Configure<KafkaClientOptions>(hostContext.Configuration.GetSection("KafkaClient"));
                    services.Configure<AsyncApiOptions>(hostContext.Configuration.GetSection("AsyncApi"));
                }).Build();

            await host.Services.GetService<ChatService>().Run();
        }

        // private static IServiceProvider GetServiceProvider()
        // {
        //     var services = new ServiceCollection();
        //     services.AddScoped(typeof(ConsumerFactory<,>));
        //     services.Configure<KafkaClientOptions>(Configuration.GetSection("KafkaClient"));
        //     
        // }
    }
}