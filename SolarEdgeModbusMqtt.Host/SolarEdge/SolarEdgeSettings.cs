namespace SolarEdgeModbusMqtt.Host.SolarEdge;

public class SolarEdgeSettings
{
    public ModBusSettings? ModBus { get; set; }
}

public class ModBusSettings
{
    public string? Host { get; set; }
    public int Port { get; set; }
}