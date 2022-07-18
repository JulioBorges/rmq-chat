using System.Text.Json;

namespace RmqChat.Server.Helpers
{
    public static class SerializeExtensions
    {
        public static byte[] SerializeData<T>(this T data)
            where T : class
        {
            return JsonSerializer.SerializeToUtf8Bytes(data);
        }

        public static T? DeserializeData<T>(this byte[] data)
            where T : class
        {
            var readOnlySpan = new ReadOnlySpan<byte>(data);
            return JsonSerializer.Deserialize<T>(readOnlySpan);
        }
    }
}
