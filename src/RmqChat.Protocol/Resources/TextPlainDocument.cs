namespace RmqChat.Protocol.Resources
{
    public class TextPlainDocument : Document
    {
        public TextPlainDocument() : base("application/txt")
        {
            Text = "";
        }

        public string Text { get; set; }

        public override string ToString() => Text;

        public static TextPlainDocument Parse(string value) => new() { Text = value };

        public static implicit operator string(TextPlainDocument textDoc) => textDoc.Text;
        public static implicit operator TextPlainDocument(string textDoc) => new () { Text = textDoc };
    }
}
