namespace YouTubeChatBot.Services
{
    public class Response
    {
        public readonly byte[] Data;
        public readonly int StatusCode;
        public Response(byte[] data,int statusCode)
        {
            Data = data;
            StatusCode = statusCode;
        }
    }
}
