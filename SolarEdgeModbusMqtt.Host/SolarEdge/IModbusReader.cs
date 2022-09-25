namespace SolarEdgeModbusMqtt.Host.SolarEdge;

public interface IModbusReader : IDisposable
{
    void Connect();
    void Disconnect();
    Task<Inverter> ReadInverterValues();
}