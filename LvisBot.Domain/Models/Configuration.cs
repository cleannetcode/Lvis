namespace LvisBot.Domain.Models
{
    public class Configuration
    {
        public string AbsBaseFolderPath { get; set; }
        public string DateTimeFormat { get; set; }
        public char KeyPrefix { get; set; }
        public YouTubeConfig YouTubeConfig { get; set; }
        public SaveModuleConfiguration TimeCodeConfigure { get; set; }
        public SaveModuleConfiguration QuestionConfigure { get; set; }
        public SaveModuleConfiguration CheckConfigure { get; set; }
        
        public SaveModuleConfiguration NewMemberConfigure { get; set; }
    }
}
