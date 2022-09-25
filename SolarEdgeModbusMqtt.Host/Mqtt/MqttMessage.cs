namespace SolarEdgeModbusMqtt.Host.Mqtt;

public record MqttMessage(string Topic, string Payload);