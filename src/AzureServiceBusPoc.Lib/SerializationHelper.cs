using System;
using System.Text;
using AzureServiceBusPoc.Lib.Configuration;
using Newtonsoft.Json;

namespace AzureServiceBusPoc.Lib
{
    public static class SerializationHelper
    {
        public static byte[] GetJsonBytes(object obj)
        {
            var json = JsonConvert.SerializeObject(obj,
                new JsonSerializerSettings {TypeNameHandling = TypeNameHandling.All});
            return Encoding.UTF8.GetBytes(json);
        }
        
        public static object GetObject(byte[] message)
        {
            var json = Encoding.UTF8.GetString(message);
            return JsonConvert.DeserializeObject(json,
                new JsonSerializerSettings {TypeNameHandling = TypeNameHandling.All});
        }

        public static object GetObject(byte[] message, Type enclosedType)
        {
            var json = Encoding.UTF8.GetString(message);
            return JsonConvert.DeserializeObject(json, enclosedType);
        }
    }
}