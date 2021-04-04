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
        FileService fileService;
        SerializeService serializeService;
        string path;
        protected abstract string savefileformat { get; }

        public BaseYTModule(FileService fileService, SerializeService serializeService, SaveModuleConfiguration moduleConfiguration)
        {
            this.fileService = fileService;
            this.serializeService = serializeService;
            Prefix = moduleConfiguration.KeyWord;
            path = moduleConfiguration.SavePath;
        }

        public string Prefix { get; }

        public void Execute(YTMessageResponse param)
        {
            var parse = param.Context.Remove(0, param.Context.IndexOf(' ') + 1);
            var model = new BaseSaveModel
            {
                DateTime = param.UtcTime,
                Message = parse,
                UserName = param.UserName,
                SecondFromStreamStart = (long)(param.UtcTime - param.StartStreamTime).TotalSeconds
            };
            var formatDate = param.StartStreamTime.ToString().Replace(' ', '_').Replace('.', '-').Replace(':', '-');
            fileService.Append($"{Path.Join(path, string.Format(savefileformat, formatDate))}.json", serializeService.Serialize(model));
        }
    }
}
