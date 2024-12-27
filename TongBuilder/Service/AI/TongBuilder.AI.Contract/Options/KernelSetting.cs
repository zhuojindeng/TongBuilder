namespace TongBuilder.AI.Contract.Options
{
    public class KernelSetting
    {
        public string ServiceType { get; set; } = string.Empty;

        public string ServiceId { get; set; } = string.Empty;

        public string DeploymentId { get; set; } = string.Empty;

        public string ModelId { get; set; } = string.Empty;

        public string Endpoint { get; set; } = string.Empty;

        public string ApiKey { get; set; } = string.Empty;

        public string SystemPrompt { get; set; } = "You are a friendly, intelligent, and curious assistant who is good at conversation.";

        public string Scheme { get; set; } = "http";

        public string Host { get; set; } = "localhost";

        public int Port { get; set; } = 3000;


    }
}
