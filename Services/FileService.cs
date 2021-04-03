using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace YouTubeChatBot.Services
{
    class FileService
    {
        public void Append(string path, string source, string separator = null)
        {
            File.AppendAllText(path, $"{source}{separator ?? Environment.NewLine}");
        }
        public void Write(string path, string source)
        {
            File.WriteAllText(path, source);
        }
        public string Read(string path)
        {
            if (!File.Exists(path))
            {
                File.Create(path);
                return string.Empty;
            }
            return File.ReadAllText(path);
        }
    }
}
