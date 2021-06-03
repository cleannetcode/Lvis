using LvisBot.BusinessLogic.Services;
using LvisBot.Domain.Interfaces;
using LvisBot.Domain.Models;

namespace LvisBot.BusinessLogic.Modules
{
    public class NewMemberHandler : BaseYTModule
    {
        public NewMemberHandler(
            FileService fileService, 
            SerializationService serializationService, 
            ConfigurationService configurationService) : base(fileService, serializationService, configurationService.NewMemberConfigure)
        {
        }

        protected override string SaveFileFormat => "Members";


        public override void Execute(YTMessageResponse param)
        {
            //base.Execute(param);
            

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
    }
}