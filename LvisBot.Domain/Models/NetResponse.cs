namespace LvisBot.Domain.Models
{
    public class NetResponse
    {
        public readonly byte[] Data;
        public readonly int StatusCode;
        public NetResponse(byte[] data, int statusCode)
        {
            Data = data;
            StatusCode = statusCode;
        }
    }
}
