using System;
using System.Collections.Generic;
using System.Text;
using YouTubeChatBot.Models;
using System.IO;

namespace YouTubeChatBot.Services
{
    class ConfigurationService
    {
        public string AbsBaseFolderPath { get; }
        public SaveModuleConfiguration TimeCodeConfigure { get; }
        public SaveModuleConfiguration QuestionConfigure { get; }
        public SaveModuleConfiguration CheckConfigure { get; }
        public YouTubeConfig YouTubeConfig { get; }
        public char ChatKeySymbol { get; }
        public string DateTimeFormat { get; }

        string settingsAbsPath = Path.Join(Environment.CurrentDirectory, "appsettings.json");
        public ConfigurationService(SerializeService serializeService)
        {
            if (!File.Exists(settingsAbsPath))
            {
                throw new FileNotFoundException($"cant find settings file with path: {settingsAbsPath}");
            }
            var setting = serializeService.Deserialize<Configuration>(File.ReadAllText(settingsAbsPath));
            AbsBaseFolderPath = setting.AbsBaseFolderPath;
            TimeCodeConfigure = setting.TimeCodeConfigure;
            QuestionConfigure = setting.QuestionConfigure;
            DateTimeFormat = setting.DateTimeFormat;
            ChatKeySymbol = setting.KeyPrefix;
            YouTubeConfig = setting.YouTubeConfig;
            CheckConfigure = setting.CheckConfigure;
        }
    }
}
