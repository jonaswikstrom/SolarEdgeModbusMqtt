namespace SolarEdgeModbusMqtt.Host;

[AttributeUsage(AttributeTargets.Property)]
public class ModbusSourceAttribute : Attribute
{
    public ModbusSourceAttribute(int valueOffset)
    {
        ValueOffset = valueOffset;
    }

    public ModbusSourceAttribute(int valueOffset, int scaleFactorOffset) : this(valueOffset)
    {
        ScaleFactorOffset = scaleFactorOffset;
    }

    public int ValueOffset { get; private set; }
    public int? ScaleFactorOffset { get; set; }

    public ModbusSourceTypeEnum ValueSourceType { get; set; } = ModbusSourceTypeEnum.int16;
    public ModbusSourceTypeEnum ScaleFactorSourceType { get; set; } = ModbusSourceTypeEnum.int16;
}


