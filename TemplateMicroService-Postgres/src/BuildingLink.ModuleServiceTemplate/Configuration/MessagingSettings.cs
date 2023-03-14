using System.Diagnostics.CodeAnalysis;

namespace BuildingLink.ModuleServiceTemplate.Configuration
{
    [ExcludeFromCodeCoverage]
    public class MessagingSettings
    {
        public const string SectionName = "Messaging";

        public MessagingSettings()
        {
            Broker = new ();
            Fake = new ();
        }

        public FakeSetting Fake { get; set; }

        public BrokerSettings Broker { get; set; }

        public int RetryDeliveryCount { get; set; }

        public int RetryDeliveryIntervalMs { get; set; }
    }

    public class FakeSetting
    {
        public bool IsActive { get; set; }
    }

    public class BrokerSettings
    {
        public string Host { get; set; } = string.Empty;

        public string VirtualHost { get; set; } = string.Empty;

        public string Username { get; set; } = string.Empty;

        public string Password { get; set; } = string.Empty;
    }
}
