using System;
using System.Collections.Generic;
using System.Text;

namespace YouTubeChatBot.Models
{
    class Configuration
    {
        public string AbsBaseFolderPath { get; set; }
        public string DateTimeFormat { get; set; }
        public char KeyPrefix { get; set; }
        public YouTubeConfig YouTubeConfig { get; set; }
        public SaveModuleConfiguration TimeCodeConfigure { get; set; }
        public SaveModuleConfiguration QuestionConfigure { get; set; }
        public SaveModuleConfiguration CheckConfigure { get; set; }
    }
}
