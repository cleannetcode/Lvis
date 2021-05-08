using LvisBot.BusinessLogic.Services;

namespace LvisBot.BusinessLogic.Modules
{
    public class QuestionHandler : BaseYTModule
    {
        protected override string SaveFileFormat => "Stream[{0}]Question";

        public QuestionHandler(FileService fileService,
            SerializationService serializationService,
            ConfigurationService configurationService) : base(fileService, serializationService,
            configurationService.QuestionConfigure)
        {
        }
    }
}