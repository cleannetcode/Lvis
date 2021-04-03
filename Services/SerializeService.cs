using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;

namespace YouTubeChatBot.Services
{
    class SerializeService
    {
        public virtual string Serialize<T>(T model)
        {
            return JsonSerializer.Serialize(model);
        }
        public virtual T Deserialize<T>(string model)
        {
            return JsonSerializer.Deserialize<T>(model);
        }
    }
}
