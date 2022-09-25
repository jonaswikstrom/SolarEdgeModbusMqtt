using System.Globalization;
using System.Reflection;
using Microsoft.Extensions.Logging;
using SolarEdgeModbusMqtt.Host.Mqtt;

namespace SolarEdgeModbusMqtt.Host;

public class ReadHandler<T>
{
    private readonly ILogger logger;
    private readonly ReadSettings.Value readValue;
    private readonly IMqttClient mqttClient;
    private readonly PropertyInfo propertyInfo;
    private readonly IList<decimal> values = new List<decimal>();

    public ReadHandler(ILogger logger, ReadSettings.Value readValue, IMqttClient mqttClient)
    {
        this.logger = logger;
        this.readValue = readValue;
        this.mqttClient = mqttClient;

        propertyInfo = typeof(T).GetProperty(readValue.Name ?? 
                                             throw new InvalidOperationException("Missing property name")) ?? 
                       throw new InvalidOperationException($"No property named {readValue.Name} for {typeof(T).Name}");
    }

    public async Task Read(T source, CancellationToken cancellationToken)
    {
        var value = propertyInfo.GetValue(source);
        if (value == null)
        {
            logger.LogWarning("No value returned for {value}", readValue.Name);
            return;
        }

        var decimalValue = Convert.ToDecimal(value);

        if (values.Count < readValue.Interval)
        {
            logger.LogDebug("Added {value} to list of {listnbr}/{interval} items", decimalValue, values.Count+1, readValue.Interval);
            values.Add(decimalValue);
            return;
        }

        var meanValue = (int)values.Sum()/values.Count;
        await mqttClient.Publish(new MqttMessage(readValue.Topic ?? string.Empty, meanValue.ToString()), cancellationToken);
        values.Clear();
    }
}