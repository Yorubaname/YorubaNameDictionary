namespace Infrastructure.Configuration
{
    public record SqsSettings(string QueueName, string Region, FrequencySettings FrequencySeconds)
    {
        public SqsSettings(): this(string.Empty, string.Empty, new FrequencySettings(default))
        {

        }
    }

    public record FrequencySettings(ushort PostPublishedNameCommand)
    {
    }
}
