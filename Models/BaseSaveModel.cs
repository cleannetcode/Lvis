﻿using System;
using System.Collections.Generic;
using System.Text;

namespace YouTubeChatBot.Models
{
    class BaseSaveModel
    {
        public DateTime DateTime { get; set; }
        public int SecondFromStreamStart { get; set; }
        public string UserName { get; set; }
        public string Message { get; set; }
    }
}
