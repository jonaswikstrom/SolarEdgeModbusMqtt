using System.Reflection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using ModbusTcp;

namespace SolarEdgeModbusMqtt.Host.SolarEdge;

public class SolarEdgeModbusReader : IModbusReader
{
    private readonly ILogger<SolarEdgeModbusReader> logger;
    private readonly IOptions<SolarEdgeSettings> settings;
    private readonly ModbusClient modbusClient;
    private readonly (PropertyInfo PropertyInfo, ModbusSourceAttribute ModbusSourceAttribute)[] inverterProperties;

    public SolarEdgeModbusReader(ILogger<SolarEdgeModbusReader> logger, IOptions<SolarEdgeSettings> settings)
    {
        this.logger = logger;
        this.settings = settings;
        if (settings.Value == null) throw new InvalidOperationException("Missing ModBus settings");
        modbusClient = new ModbusClient(settings.Value.ModBus?.Host, settings.Value!.ModBus!.Port);
        inverterProperties = GetUpdateableProperties<Inverter>().ToArray();
    }

    public void Dispose() => modbusClient.Terminate();

    public void Connect()
    {
        logger.LogInformation("Connecting to {host}", $"{settings.Value.ModBus!.Host}:{settings.Value.ModBus.Port}");
        modbusClient.Init();
        logger.LogInformation("Connected");
    }

    public void Disconnect()
    {
        logger.LogInformation("Disconnecting ModBus connection");
        modbusClient.Terminate();
        logger.LogInformation("Disconnected");
    }

    public async Task<Inverter> ReadInverterValues()
    {
        logger.LogDebug("Reading inverter values");
        var inverterResponse = await modbusClient.ReadRegistersAsync(40069, 48);
        var inverter = new Inverter();

        foreach (var property in inverterProperties)
        {
            var value = GetModbusValue(inverterResponse, 40069, property.ModbusSourceAttribute);
            property.PropertyInfo.SetValue(inverter, Convert.ChangeType(value, property.PropertyInfo.PropertyType));
        }

        logger.LogDebug("Done reading {propertiescount} preoperties for inverter", inverterProperties.Length);
        return inverter;
    }

    private static object? GetModbusValue(IReadOnlyList<short> modbusDataArray, int arrayOffset, ModbusSourceAttribute modbusSource)
    {
        if (modbusSource.ValueOffset - arrayOffset < 0 || (modbusSource.ValueOffset - arrayOffset) >= modbusDataArray.Count)
        {
            return null;
        }

        double numericValue = modbusSource.ValueSourceType switch
        {
            ModbusSourceTypeEnum.int16 => modbusDataArray[modbusSource.ValueOffset - arrayOffset],
            ModbusSourceTypeEnum.uint32 => BitConverter.ToUInt16(
                                               BitConverter.GetBytes(modbusDataArray[modbusSource.ValueOffset - arrayOffset]), 0) * 65536 +
                                           BitConverter.ToUInt16(BitConverter.GetBytes(modbusDataArray[modbusSource.ValueOffset - arrayOffset + 1]), 0),
            ModbusSourceTypeEnum.uint16 => BitConverter.ToUInt16(BitConverter.GetBytes(modbusDataArray[modbusSource.ValueOffset - arrayOffset]), 0),
            _ => throw new Exception($"Unknown {nameof(ModbusSourceTypeEnum)} {modbusSource.ValueSourceType} for {nameof(modbusSource.ValueSourceType)}")
        };

        if (!modbusSource.ScaleFactorOffset.HasValue)
        {
            return numericValue;
        }

        if (modbusSource.ScaleFactorOffset.Value - arrayOffset < 0 || modbusSource.ScaleFactorOffset.Value - arrayOffset >= modbusDataArray.Count)
        {
            return null;
        }

        double numericScale = modbusSource.ScaleFactorSourceType switch
        {
            ModbusSourceTypeEnum.int16 => modbusDataArray[modbusSource.ScaleFactorOffset.Value - arrayOffset],
            ModbusSourceTypeEnum.uint32 => (uint)modbusDataArray[modbusSource.ScaleFactorOffset.Value - arrayOffset] *
                65536 + (uint)modbusDataArray[modbusSource.ScaleFactorOffset.Value - arrayOffset + 1],
            ModbusSourceTypeEnum.uint16 => modbusDataArray[modbusSource.ScaleFactorOffset.Value - arrayOffset],
            _ => throw new Exception($"Unknown {nameof(ModbusSourceTypeEnum)} {modbusSource.ScaleFactorSourceType}")
        };

        return numericValue * Math.Pow(10, numericScale);
    }

    private static IEnumerable<(PropertyInfo PropertyInfo, ModbusSourceAttribute ModbusSourceAttribute)> GetUpdateableProperties<TType>()
    {
        foreach (var propertyInfo in typeof(TType).GetProperties())
        {
            var modbusSourceAttribute = propertyInfo.GetCustomAttribute<ModbusSourceAttribute>();
            if (modbusSourceAttribute != null) yield return (propertyInfo, modbusSourceAttribute);
        }
    }
}