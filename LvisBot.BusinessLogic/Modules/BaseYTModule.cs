using System.IO;
using LvisBot.BusinessLogic.Services;
using LvisBot.Domain.Interfaces;
using LvisBot.Domain.Models;

namespace LvisBot.BusinessLogic.Modules
{
    public abstract class BaseYTModule : IActionPrefixModule<YTMessageResponse, string>
    {
        protected readonly IFileService FileService;
        protected readonly ISerializationService SerializationService;
        protected readonly string Path;
        protected abstract string SaveFileFormat { get; }

        public BaseYTModule(IFileService fileService, ISerializationService serializationService, SaveModuleConfiguration moduleConfiguration)
        {
            FileService = fileService;
            SerializationService = serializationService;
            Prefix = moduleConfiguration.KeyWord;
            Path = moduleConfiguration.SavePath;
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
            var filePath = $"{System.IO.Path.Join(Path, string.Format(SaveFileFormat, dateFormat))}.json";

            FileService.Append(filePath, SerializationService.Serialize(model));
        }
    }
}
