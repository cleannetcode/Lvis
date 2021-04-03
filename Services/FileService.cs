using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace YouTubeChatBot.Services
{
    class FileService
    {
        public void Write(string source, string path)
        {
            File.WriteAllText(source, path);
        }
        public string Read(string path)
        {
            return File.ReadAllText(path);
        }
    }
}
