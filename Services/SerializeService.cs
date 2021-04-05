using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;

namespace YouTubeChatBot.Services
{
    class SerializeService
    {
        public virtual string Serialize<T>(T model)
        {
            return JsonConvert.SerializeObject(model);
        }
        public virtual T Deserialize<T>(string model)
        {
            return JsonConvert.DeserializeObject<T>(model);
        }
    }
}
