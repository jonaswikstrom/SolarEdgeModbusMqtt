using System.Timers;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SolarEdgeModbusMqtt.Host.Mqtt;
using SolarEdgeModbusMqtt.Host.SolarEdge;

namespace SolarEdgeModbusMqtt.Host;

public class HostService : IHostedService
{
    private readonly ILogger<HostService> logger;
    private readonly IHostApplicationLifetime applicationLifetime;
    private readonly IMqttClient mqttClient;
    private readonly IModbusReader modbusReader;
    private readonly IOptions<ReadSettings> readSettings;

    public HostService(ILogger<HostService> logger, IHostApplicationLifetime applicationLifetime, 
        IMqttClient mqttClient, IModbusReader modbusReader, IOptions<ReadSettings> readSettings)
    {
        this.logger = logger;
        this.applicationLifetime = applicationLifetime;
        this.mqttClient = mqttClient;
        this.modbusReader = modbusReader;
        this.readSettings = readSettings;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        Environment.ExitCode = 0;
        logger.LogInformation("Host starting");

        modbusReader.Connect();
        await mqttClient.Connect(cancellationToken);

        var inverterReadHandlers = (readSettings.Value.Values ?? throw new InvalidOperationException("Missing read values"))
            .Where(v => v.Name != null && v.Unit != null && v.Unit!.Equals("inverter", StringComparison.InvariantCultureIgnoreCase))
            .Select(rh => new ReadHandler<Inverter>(logger, rh, mqttClient)).ToList();

        applicationLifetime.ApplicationStarted.Register(() =>
        {
            Task.Run(async () =>
            {
                try
                {
                    do
                    {
                        var inverter = await modbusReader.ReadInverterValues();
                        var inverterTasks = inverterReadHandlers.Select(s => s.Read(inverter, cancellationToken));
                        await Task.WhenAll(inverterTasks);

                        await Task.Delay(1000, cancellationToken);

                    } while (!cancellationToken.IsCancellationRequested);

                }
                catch (Exception e)
                {
                    Environment.ExitCode = -1;
                    logger.LogError(e, "Unhandeled exception");
                }
                finally
                {
                    applicationLifetime.StopApplication();
                }
            }, cancellationToken);
        });
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}