using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace YouTubeChatBot.Services
{
    class FileService
    {
        string absBaseFolderPath;
        object locker = new object();
        //List<Locker> fileAccess = new List<Locker>();
        public FileService(ConfigurationService configurationService)
        {
            absBaseFolderPath = configurationService.AbsBaseFolderPath;
        }
        public void Append(string path, string source, string separator = null)
        {
            lock (locker)
            {
                var path1 = Path.Join(absBaseFolderPath, path);
                File.AppendAllText(path1, $"{source}{separator ?? Environment.NewLine}");
            }
        }
        public void Write(string path, string source)
        {
            lock (locker)
            {
                File.WriteAllText(Path.Join(absBaseFolderPath, path), source);
            }
        }
        public string Read(string path)
        {
            lock (locker)
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
        //class Locker
        //{
        //    public object locker = new object();
        //    public string file;
        //    public Locker(string path)
        //    {
        //        file = path;
        //    }
        //    public override bool Equals(object obj)
        //    {
        //        var locker = obj as Locker;
        //        if (locker == null) return false;
        //        return locker.file == file;
        //    }
        //    public override int GetHashCode()
        //    {
        //        return file.GetHashCode();
        //    }
        //}
    }
}
