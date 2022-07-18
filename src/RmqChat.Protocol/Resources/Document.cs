namespace RmqChat.Protocol.Resources
{
    public class Document
    {
        public Guid Id { get; set; }

        public string Type { get; set; }

        protected Document(string type)
        {
            Id = Guid.NewGuid();
            Type = type;
        }
    }
}
