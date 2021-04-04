using System;
namespace YouTubeChatBot
{
    class Program
    {
        static void Main(string[] args)
        {
            using (var startup = new Startup())
            {
                startup.Run();
            }
        }
    }
}
