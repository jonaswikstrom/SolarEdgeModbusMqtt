using System.Text;

namespace SolarEdgeModbusMqtt.Host.Mqtt;

public class MqttSettings
{
    public string? Host { get; set; }
    public int? Port { get; set; }
    public string? Username { get; set; }
    public string? Password { get; set; }
    public string? TopicRoot { get; set; }

    public string FullHostName()
    {
        var sb = new StringBuilder(Host);
        if (Port != null) sb.Append($":{Port.Value}");
        return sb.ToString();
    }
}