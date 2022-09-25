using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using SolarEdgeModbusMqtt.Host.Mqtt;
using SolarEdgeModbusMqtt.Host.SolarEdge;

namespace SolarEdgeModbusMqtt.Host;
public class Program
{
    public static async Task Main(string[] args)
    {
        var cts = new CancellationTokenSource();

        var configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: false, true)
            .AddEnvironmentVariables()
            .Build();

        var hostBuilder = new HostBuilder().ConfigureServices(p =>
        {
            p.AddLogging(loggingBuilder => loggingBuilder.AddConsole())
                .Configure<SolarEdgeSettings>(configuration.GetSection("SolarEdge"))
                .Configure<ReadSettings>(configuration.GetSection("Read"))
                .Configure<MqttSettings>(configuration.GetSection("Mqtt"))
                .AddSingleton<IMqttClient, MqttClient>()
                .AddSingleton<IModbusReader, SolarEdgeModbusReader>()
                .AddHostedService<HostService>();
        });


        await hostBuilder.RunConsoleAsync(cts.Token);
    }
}