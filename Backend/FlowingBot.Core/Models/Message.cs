namespace FlowingBot.Core.Models
{
    public class Message : EntityBase
    {
        public Chat Chat { get; set; }
        public int ChatId { get; set; }
        public RoleEnum Role { get; set; }
        public string Text { get; set; }
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    }

    public enum RoleEnum
    {
        Assistant = 1,
        User = 2,
        System = 3
    }
}