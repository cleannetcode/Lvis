using System;
using System.Collections.Generic;
using System.Text;
using YouTubeChatBot.Services;
using YouTubeChatBot.Models;
using System.IO;
using YouTubeChatBot.Interfaces;

namespace YouTubeChatBot.Modules
{
    abstract class BaseYTModule : IActionPrefixModule<YTMessageResponse, string>
    {
        protected readonly FileService fileService;
        protected readonly SerializeService serializeService;
        protected readonly string path;
        protected abstract string savefileformat { get; }

        public BaseYTModule(FileService fileService, SerializeService serializeService, SaveModuleConfiguration moduleConfiguration)
        {
            this.fileService = fileService;
            this.serializeService = serializeService;
            Prefix = moduleConfiguration.KeyWord;
            path = moduleConfiguration.SavePath;
        }

        public string Prefix { get; }

        public virtual void Execute(YTMessageResponse param)
        {
            var removeLength = Prefix.Length + 2;
            var parse = param.Context.Remove(0, removeLength < param.Context.Length ? removeLength : param.Context.Length);
            var model = new BaseSaveModel
            {
                DateTime = param.UtcTime,
                Message = parse,
                UserName = param.UserName,
                SecondFromStreamStart = (long)(param.UtcTime - param.StartStreamTime).TotalSeconds,
                TimeSpan = (param.UtcTime - param.StartStreamTime).ToString(@"hh\:mm\:ss")
            };
            var dateFormat = param.StartStreamTime.ToString("dd.MM.yyyy_HH.mm.ss");
            SaveFile(model, dateFormat);
        }
        protected void SaveFile<T>(T model, string dateFormat)
        {
            var filePath = $"{Path.Join(path, string.Format(savefileformat, dateFormat))}.json";

            fileService.Append(filePath, serializeService.Serialize(model));
        }
    }
}
