using RmqChat.Protocol.Resources;

namespace RmqChat.Protocol.Messaging
{
    public class Message
    {
        public Guid Id { get; set; }
        public string? From { get; set; }
        public TextPlainDocument Resource { get; set; }

        public Message()
        {
            Id = Guid.NewGuid();
        }
    }
}
