using LvisBot.BusinessLogic.Services;
using LvisBot.Domain.Interfaces;

namespace LvisBot.BusinessLogic.Modules
{
    public class TimeCodeHandler : BaseYTModule
    {
        protected override string SaveFileFormat => "Stream[{0}]TimeCodes";

        public TimeCodeHandler(IFileService fileService,
            ISerializationService serializationService,
            ConfigurationService configurationService) : base(fileService, serializationService,
            configurationService.TimeCodeConfigure)
        {
        }
    }
}