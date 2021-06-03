using System;
using System.IO;
using LvisBot.Domain.Interfaces;
using LvisBot.Domain.Models;

namespace LvisBot.BusinessLogic.Services
{
    public class ConfigurationService
    {
        public string AbsBaseFolderPath { get; }
        public SaveModuleConfiguration TimeCodeConfigure { get; }
        public SaveModuleConfiguration QuestionConfigure { get; }
        public SaveModuleConfiguration CheckConfigure { get; }
        public YouTubeConfig YouTubeConfig { get; }
        public char ChatKeySymbol { get; }
        public string DateTimeFormat { get; }
        public string FaleNameMembers { get; }

        string settingsAbsPath = Path.Join(Environment.CurrentDirectory, "appsettings.json");
        public ConfigurationService(ISerializationService serializationService)
        {
            if (!File.Exists(settingsAbsPath))
            {
                throw new FileNotFoundException($"cant find settings file with path: {settingsAbsPath}");
            }
            var setting = serializationService.Deserialize<Configuration>(File.ReadAllText(settingsAbsPath));
            AbsBaseFolderPath = setting.AbsBaseFolderPath;
            TimeCodeConfigure = setting.TimeCodeConfigure;
            QuestionConfigure = setting.QuestionConfigure;
            DateTimeFormat = setting.DateTimeFormat;
            ChatKeySymbol = setting.KeyPrefix;
            YouTubeConfig = setting.YouTubeConfig;
            CheckConfigure = setting.CheckConfigure;
            FaleNameMembers = setting.FaleNameMembers;
        }

        public SaveModuleConfiguration NewMemberConfigure { get; }
    }
}
