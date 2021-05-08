using LvisBot.BusinessLogic.Services;

namespace LvisBot.BusinessLogic.Modules
{
    public class CheckHandler : BaseYTModule
    {
        protected override string SaveFileFormat => "Stream[{0}]Checks";
        public CheckHandler(FileService fileService, SerializationService serializationService, ConfigurationService configurationService)
            : base(fileService, serializationService, configurationService.CheckConfigure) { }
    }
}
