using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace YouTubeChatBot.Services
{
    class FileService
    {
        string absBaseFolderPath;
        public FileService(ConfigurationService configurationService)
        {
            absBaseFolderPath = configurationService.AbsBaseFolderPath;
        }
        public void Append(string path, string source, string separator = null)
        {
            path = Path.Join(absBaseFolderPath, path);
            if (!File.Exists(path))
            {
                File.Create(path);
            }
            File.AppendAllText(path, $"{source}{separator ?? Environment.NewLine}");
        }
        public void Write(string path, string source)
        {
            File.WriteAllText(Path.Join(absBaseFolderPath, path), source);
        }
        public string Read(string path)
        {
            path = Path.Join(absBaseFolderPath, path);
            if (!File.Exists(path))
            {
                File.Create(path);
                return string.Empty;
            }
            return File.ReadAllText(path);
        }
    }
}
