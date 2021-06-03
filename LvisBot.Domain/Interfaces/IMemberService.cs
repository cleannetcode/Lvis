using LvisBot.Domain.Models;

namespace LvisBot.BusinessLogic.Services
{
    public interface IMemberService
    {
        Member[] GetAllMembers();
        bool ChechUniqueMember(Member user);
    }
}