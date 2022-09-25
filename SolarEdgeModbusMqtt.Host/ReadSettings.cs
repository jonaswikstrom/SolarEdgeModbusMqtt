namespace SolarEdgeModbusMqtt.Host;

public class ReadSettings
{
    public Value[]? Values { get; set; }

    public class Value
    {
        public string? Unit { get; set; }
        public string? Name { get; set; }
        public int Interval { get; set; }
        public string? Topic { get; set; }
    }
}