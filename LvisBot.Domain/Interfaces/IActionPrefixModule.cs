namespace LvisBot.Domain.Interfaces
{
    public interface IActionPrefixModule<TMess,TPrefix> : IActionModule<TMess>
    {
        TPrefix Prefix { get; }
    }
}
