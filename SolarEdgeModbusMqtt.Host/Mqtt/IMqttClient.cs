namespace SolarEdgeModbusMqtt.Host.Mqtt;

public interface IMqttClient : IDisposable
{
    Task Connect(CancellationToken cancellationToken = default);
    Task Publish(MqttMessage mqttMessage, CancellationToken cancellationToken = default);
    Task Disconnect(CancellationToken cancellationToken = default);

    Task Subscribe(string topic);
    Task UnSubscribe(string topic);

    event Func<MqttMessage, Task>? OnMessageReceived;

    string TopicRoot { get; }
}