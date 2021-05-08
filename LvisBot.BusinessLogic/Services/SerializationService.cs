using LvisBot.Domain.Interfaces;
using Newtonsoft.Json;

namespace LvisBot.BusinessLogic.Services
{
    public class SerializationService : ISerializationService
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
