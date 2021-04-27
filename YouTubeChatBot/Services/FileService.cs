using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace YouTubeChatBot.Services
{
    public class FileService
    {
        private string absBaseFolderPath;
        //в идеале иногда чистить этот словарь, но скорее всего нужно вообще иначе реализовывать доступ
        //еще было бы неплохо асинхронные методы добавить
        Dictionary<string, object> fileAcess = new Dictionary<string, object>();
        public FileService(ConfigurationService configurationService)
        {
            absBaseFolderPath = configurationService.AbsBaseFolderPath;
        }
        public void Append(string path, string source, string separator = null)
        {
            lock (GetLocker(path))
            {
                var path1 = Path.Join(absBaseFolderPath, path);
                File.AppendAllText(path1, $"{source}{separator ?? Environment.NewLine}");
            }
        }
        public void Write(string path, string source)
        {
            lock (GetLocker(path))
            {
                File.WriteAllText(Path.Join(absBaseFolderPath, path), source);
            }
        }
        public string Read(string path)
        {
            lock (GetLocker(path))
            {
                var path1 = Path.Join(absBaseFolderPath, path);
                if (!File.Exists(path1))
                {
                    File.Create(path);
                    return string.Empty;
                }
                return File.ReadAllText(path1);
            }
        }
        private object GetLocker(string path)
        {
            if(fileAcess.TryGetValue(path, out var locker))
            {
                return locker;
            }
            else
            {
                var obj = new object();
                fileAcess.Add(path, obj);
                return obj;
            }
        }
    }
}
