namespace LvisBot.Domain.Interfaces
{
    public interface IFileService
    {
        void Append(string path, string source, string separator = null);
        string[] ReadAllText(string path);
    }
}