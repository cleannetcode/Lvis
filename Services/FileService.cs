using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.IO;

namespace YouTubeChatBot.Services
{
    class FileService
    {
        public void Write(string source, string path)
        {
            File.WriteAllText(path, source);
        }
        public string Read(string path)
        {
            return File.ReadAllText(path);
        }
    }
}
