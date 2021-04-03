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
            using (FileStream fstream = new FileStream($"{path}\note.txt", FileMode.OpenOrCreate))
            {
                // преобразуем строку в байты
                byte[] array = System.Text.Encoding.Default.GetBytes(source);
                // запись массива байтов в файл
                fstream.Write(array, 0, array.Length);
                Console.WriteLine("Текст записан в файл");
            }
        }
        public string Read(string path)
        {
            using (FileStream fstream = File.OpenRead($"{path}\note.txt"))
            {
                // преобразуем строку в байты
                byte[] array = new byte[fstream.Length];
                // считываем данные
                fstream.Read(array, 0, array.Length);
                // декодируем байты в строку
                string textFromFile = System.Text.Encoding.Default.GetString(array);
                return textFromFile;
            }
        }
    }
}
