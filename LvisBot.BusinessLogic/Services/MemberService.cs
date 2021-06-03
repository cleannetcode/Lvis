using System.Collections.Generic;
using System.IO;
using LvisBot.Domain.Interfaces;
using LvisBot.Domain.Models;

namespace LvisBot.BusinessLogic.Services
{
    public class MemberService : IMemberService
    {
        private List<Member> _currentSessionMembers = new List<Member>();
        
        private IFileService _fileService;
        private ISerializationService _serializationService;
        private string _fileName;
        
        public MemberService(
            IFileService fileService,
            ISerializationService serializationService, 
            ConfigurationService configurationService)
        {
            _fileService = fileService;
            _serializationService = serializationService;
            _fileName = configurationService.FaleNameMembers;
        }

        private string[] ReadDataMembers()
        {
            return _fileService.ReadAllText(
                $"{System.IO.Path.Join(_fileName)}.json");
        }
        
        private void SaveDadaMember(Member member)
        {
            var filePath = $"{System.IO.Path.Join(_fileName)}.json";
            _fileService.Append(filePath, _serializationService.Serialize(member));
        }
        
        public Member[] GetAllMembers()
        {
            var listModels = new List<Member>();
            foreach (var line in ReadDataMembers())
            {
                var savedModel = _serializationService.Deserialize<Member>(line);
                listModels.Add(savedModel);
            }
            return listModels.ToArray();
        }

        public bool ChechUniqueMember(Member user)
        {
            foreach (var sessionMember in _currentSessionMembers)
            {
                if (sessionMember.UserName == user.UserName)
                {
                    return true;
                }
            }

            var savedData = ReadDataMembers();
            if (savedData != null)
            {
                foreach (var line in savedData)
                {
                    var member = _serializationService.Deserialize<Member>(line);
                    if (member.UserName == user.UserName)
                    {
                        _currentSessionMembers.Add(member);
                        return true;
                    }
                }
            }

            _currentSessionMembers.Add(user);
            SaveDadaMember(user);
            return false;
        }
        
    }
}