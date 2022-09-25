namespace SolarEdgeModbusMqtt.Host.SolarEdge;

public class Inverter
{
    /// <summary>
    /// AC Total Current value in Amps
    /// </summary>
    [ModbusSource(40072 + Constants.InverterBaseOffset, 40076 + Constants.InverterBaseOffset, ValueSourceType = ModbusSourceTypeEnum.uint16)]
    public float AcCurrent { get; set; } = -1;

    /// <summary>
    /// AC Total Current value in Amps
    /// </summary>
    [ModbusSource(40073 + Constants.InverterBaseOffset, 40076 + Constants.InverterBaseOffset, ValueSourceType = ModbusSourceTypeEnum.uint16)]
    public float ACCurrentAA { get; set; } = -1;


    /// <summary>
    /// AC Phase B Current value in Amps
    /// </summary>
    [ModbusSource(40074 + Constants.InverterBaseOffset, 40076 + Constants.InverterBaseOffset, ValueSourceType = ModbusSourceTypeEnum.uint16)]
    public float ACCurrentAB { get; set; } = -1;

    /// <summary>
    /// AC Phase C Current value in Amps
    /// </summary>
    [ModbusSource(40075 + Constants.InverterBaseOffset, 40076 + Constants.InverterBaseOffset, ValueSourceType = ModbusSourceTypeEnum.uint16)]
    public float ACCurrentAC { get; set; } = -1;

    /// <summary>
    /// AC Voltage Phase AB value in Volts
    /// </summary>
    [ModbusSource(40077 + Constants.InverterBaseOffset, 40083 + Constants.InverterBaseOffset, ValueSourceType = ModbusSourceTypeEnum.uint16)]
    public float ACVoltageAB { get; set; } = -1;

    /// <summary>
    /// AC Voltage Phase BC value in Volts
    /// </summary>
    [ModbusSource(40078 + Constants.InverterBaseOffset, 40083 + Constants.InverterBaseOffset, ValueSourceType = ModbusSourceTypeEnum.uint16)]
    public float ACVoltageBC { get; set; } = -1;

    /// <summary>
    /// AC Voltage Phase CA value in Volts
    /// </summary>
    [ModbusSource(40079 + Constants.InverterBaseOffset, 40083 + Constants.InverterBaseOffset, ValueSourceType = ModbusSourceTypeEnum.uint16)]
    public float ACVoltageCA { get; set; } = -1;

    /// <summary>
    /// AC Voltage Phase A to N value in Volts
    /// </summary>
    [ModbusSource(40080 + Constants.InverterBaseOffset, 40083 + Constants.InverterBaseOffset, ValueSourceType = ModbusSourceTypeEnum.uint16)]
    public float ACVoltageAN { get; set; } = -1;

    /// <summary>
    /// AC Voltage Phase B to N value in Volts
    /// </summary>
    [ModbusSource(40081 + Constants.InverterBaseOffset, 40083 + Constants.InverterBaseOffset, ValueSourceType = ModbusSourceTypeEnum.uint16)]
    public float ACVoltageBN { get; set; } = -1;

    /// <summary>
    /// AC Voltage Phase C to N value in Volts
    /// </summary>
    [ModbusSource(40082 + Constants.InverterBaseOffset, 40083 + Constants.InverterBaseOffset, ValueSourceType = ModbusSourceTypeEnum.uint16)]
    public float ACVoltageCN { get; set; } = -1;

    /// <summary>
    /// AC Power value in Watts
    /// </summary>
    [ModbusSource(40084 + Constants.InverterBaseOffset, 40085 + Constants.InverterBaseOffset, ValueSourceType = ModbusSourceTypeEnum.uint16)]
    public float ACPower { get; set; } = -1;

    /// <summary>
    /// AC Frequency value in Hertz
    /// </summary>
    [ModbusSource(40086 + Constants.InverterBaseOffset, 40087 + Constants.InverterBaseOffset)]
    public float ACFrequency { get; set; } = -1;

    /// <summary>
    /// Apparent Power in Volt-Amps
    /// </summary>
    [ModbusSource(40088 + Constants.InverterBaseOffset, 40089 + Constants.InverterBaseOffset)]
    public float ACVA { get; set; } = -1;

    /// <summary>
    /// Reactive Power in VAR
    /// </summary>
    [ModbusSource(40090 + Constants.InverterBaseOffset, 40091 + Constants.InverterBaseOffset)]
    public float ACVAR { get; set; } = -1;

    /// <summary>
    /// Power Factor in %
    /// </summary>
    [ModbusSource(40092 + Constants.InverterBaseOffset, 40093 + Constants.InverterBaseOffset)]
    public float ACPowerFactor { get; set; } = -1;

    /// <summary>
    /// AC Lifetime Energy production in WattHours
    /// </summary>
    [ModbusSource(40094 + Constants.InverterBaseOffset, 40096 + Constants.InverterBaseOffset, ValueSourceType = ModbusSourceTypeEnum.uint32, ScaleFactorSourceType = ModbusSourceTypeEnum.uint16)]
    public float ACLifetimeEnergy { get; set; } = -1;

    /// <summary>
    /// DC Current value in Amps
    /// </summary>
    [ModbusSource(40097 + Constants.InverterBaseOffset, 40098 + Constants.InverterBaseOffset, ValueSourceType = ModbusSourceTypeEnum.uint16)]
    public float DCCurrent { get; set; } = -1;

    /// <summary>
    /// DC Voltage value in Volts
    /// </summary>
    [ModbusSource(40099 + Constants.InverterBaseOffset, 40100 + Constants.InverterBaseOffset, ValueSourceType = ModbusSourceTypeEnum.uint16)]
    public float DCVoltage { get; set; } = -1;

    /// <summary>
    /// DC Power value in Watts
    /// </summary>
    [ModbusSource(40101 + Constants.InverterBaseOffset, 40102 + Constants.InverterBaseOffset)]
    public float DCPower { get; set; } = -1;

    /// <summary>
    /// Heat Sink Temperature in Degrees Celsius
    /// </summary>
    [ModbusSource(40104 + Constants.InverterBaseOffset, 40107 + Constants.InverterBaseOffset)]
    public float HeatSinkTemperature { get; set; } = -1;
}