namespace LvisBot.Domain.Interfaces
{
    //TMess - MessageResponse
    public interface IActionModule<TMess>
    {
        void Execute(TMess param);
    }
}
