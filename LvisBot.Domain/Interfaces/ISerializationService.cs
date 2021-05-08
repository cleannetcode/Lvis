namespace LvisBot.Domain.Interfaces
{
    public interface ISerializationService
    {
        string Serialize<T>(T model);
        T Deserialize<T>(string model);
    }
}