namespace FlowingBot.Core.Models
{
    public class Chat : EntityBase
    {
        public string Title { get; set; } = string.Empty;
        public string AssistantText { get; set; } = string.Empty;
        public string CollectionName { get; set; } = string.Empty;
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;

        public IList<Message> Messages { get; set; } = new List<Message>();
    }
}