using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MQTTnet;
using MQTTnet.Client;
using MQTTnet.Protocol;

namespace SolarEdgeModbusMqtt.Host.Mqtt;

public class MqttClient : IMqttClient
{
    private readonly ILogger<MqttClient> logger;
    private readonly IHostApplicationLifetime applicationLifetime;
    private readonly IOptions<MqttSettings> settings;
    private readonly MQTTnet.Client.IMqttClient client;
    private readonly MqttClientOptions options;
    public event Func<MqttMessage, Task>? OnMessageReceived;


    public MqttClient(ILogger<MqttClient> logger, IHostApplicationLifetime applicationLifetime, IOptions<MqttSettings> settings)
    {
        this.logger = logger;
        this.applicationLifetime = applicationLifetime;
        this.settings = settings;

        var factory = new MqttFactory();
        client = factory.CreateMqttClient();

        options = new MqttClientOptionsBuilder()
            .WithProtocolVersion(MQTTnet.Formatter.MqttProtocolVersion.V500)
            .WithTcpServer(settings.Value.Host, settings.Value.Port)
            .WithCredentials(settings.Value.Username, settings.Value.Password)
            .Build();
    }

    public async Task Connect(CancellationToken cancellationToken = default)
    {
        logger.LogInformation($"Connecting to {settings.Value.FullHostName()}");

        await client.ConnectAsync(options, cancellationToken);
        client.DisconnectedAsync += ClientOnDisconnectedAsync;
        logger.LogInformation("Connected");


        client.ApplicationMessageReceivedAsync += ClientOnApplicationMessageReceivedAsync;
    }

    private async Task ClientOnApplicationMessageReceivedAsync(MqttApplicationMessageReceivedEventArgs arg)
    {
        var mqttMessage = new MqttMessage(arg.ApplicationMessage.Topic, arg.ApplicationMessage.ConvertPayloadToString());
        await OnMessageReceived?.Invoke(mqttMessage)!;
    }

    private Task ClientOnDisconnectedAsync(MqttClientDisconnectedEventArgs arg)
    {
        logger.LogError(arg.Exception, "Disconnected");
        applicationLifetime.StopApplication();
        return Task.CompletedTask;
    }

    public void Dispose()
    {
        client.Dispose();
    }

    public async Task Publish(MqttMessage mqttMessage, CancellationToken cancellationToken = default)
    {
        var fullTopic = $"{settings.Value.TopicRoot}/{mqttMessage.Topic}".Replace(" ", "").Replace("//", "/");
        var message = new MqttApplicationMessageBuilder()
            .WithTopic(fullTopic)
            .WithPayload(mqttMessage.Payload)
            .WithRetainFlag()
            .Build();

        await client.PublishAsync(message, cancellationToken);
        logger.LogInformation("Published to topic '{topic}' with payload {payload}", fullTopic, mqttMessage.Payload);
    }

    public async Task Disconnect(CancellationToken cancellationToken = default)
    {
        var disconnectOptions = new MqttClientDisconnectOptions
        {
            Reason = MqttClientDisconnectReason.NormalDisconnection,
            ReasonString = "Client shutting down"
        };
        await client.DisconnectAsync(disconnectOptions, cancellationToken);
    }

    public async Task Subscribe(string topic)
    {
        var fullSubscribe = $"{settings.Value.TopicRoot}/{topic}".Replace("//", "/").ToLowerInvariant();
        logger.LogInformation($"Subscribing to topic '{topic}'", fullSubscribe);
        await client.SubscribeAsync(fullSubscribe, MqttQualityOfServiceLevel.AtLeastOnce);
    }

    public async Task UnSubscribe(string topic)
    {
        await client.UnsubscribeAsync($"{settings.Value.TopicRoot}/{topic}");
    }

    public string TopicRoot => settings.Value.TopicRoot ?? string.Empty;
}