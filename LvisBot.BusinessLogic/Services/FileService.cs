using System;
using System.Collections.Generic;
using System.IO;
using LvisBot.Domain.Interfaces;

namespace LvisBot.BusinessLogic.Services
{
    public class FileService : IFileService
    {
        private readonly string _absBaseFolderPath;

        //в идеале иногда чистить этот словарь, но скорее всего нужно вообще иначе реализовывать доступ
        //еще было бы неплохо асинхронные методы добавить
        Dictionary<string, object> fileAcess = new Dictionary<string, object>();

        public FileService(ConfigurationService configurationService)
        {
            _absBaseFolderPath = configurationService.AbsBaseFolderPath;
        }

        public void Append(string path, string source, string separator = null)
        {
            lock (GetLocker(path))
            {
                var path1 = Path.Join(_absBaseFolderPath, path);
                File.AppendAllText(path1, $"{source}{separator ?? Environment.NewLine}");
            }
        }

        public string[] ReadAllText(string path)
        {
            var path1 = Path.Join(_absBaseFolderPath, path);
            string[] result;
            try
            {
                result = File.ReadAllLines(path1);
            }
            catch
            {
                return null;
            }

            return result;
        }

        private object GetLocker(string path)
        {
            if (fileAcess.TryGetValue(path, out var locker))
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