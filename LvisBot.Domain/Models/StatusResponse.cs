namespace LvisBot.Domain.Models
{
    public class StatusResponse
    {
        /// <summary>
        /// 200 - Good
        /// 400 - Ended
        /// 404 - Not founded
        /// </summary>
        public int Code { get; }
        public string State { get; }
        public StatusResponse(int code, string state)
        {
            Code = code;
            State = state;
        }
    }
}
