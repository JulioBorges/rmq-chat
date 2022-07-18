namespace RmqChat.Protocol.Messaging
{
    public class Command
    {
        public Guid Id { get; set; }
        public string? From { get; set; }
        public string CommandText { get; set; }
        public string CommandArgs { get; set; }

        public Command()
        {
            Id = Guid.NewGuid();
            CommandText = string.Empty;
            CommandArgs = string.Empty;
        }
    }
}
