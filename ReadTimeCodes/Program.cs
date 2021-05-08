using LvisBot.DataAccess;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LvisBot.DataAccess.Entities;

namespace ReadTimeCodes
{
    internal class Program
    {
        private static async Task Main(string[] args)
        {
            Console.OutputEncoding = Encoding.UTF8;

            var lines = await File.ReadAllLinesAsync(@"E:\projects\YouTubeChatBot\bin\Debug\netcoreapp3.1\Resources\Stream[25.04.2021_11.58.06]TimeCodes.json");

            var timecodes = new List<BaseSaveModel>();

            foreach (var line in lines)
            {
                var timecode = JsonConvert.DeserializeObject<BaseSaveModel>(line);

                timecodes.Add(timecode);
            }

            timecodes = timecodes.Distinct().ToList();

            foreach (var timecode in timecodes)
            {
                Console.WriteLine($"{timecode.UserName}, {timecode.TimeSpan}, {timecode.Message}");
            }
        }
    }
}
