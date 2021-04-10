using System;
using System.Collections.Generic;
using System.Text;

namespace YouTubeChatBot.Models
{
    class CheckSaveModel
    {
        public DateTime DateTime { get; set; }
        public long SecondFromStreamStart { get; set; }
        public string UserName { get; set; }
        public string Comment { get; set; }
    }
}
