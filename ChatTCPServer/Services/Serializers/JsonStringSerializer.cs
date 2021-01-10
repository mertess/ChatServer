using Newtonsoft.Json;
using System.Text.Json;
using ServerBusinessLogic.Interfaces;
using System.Text.Encodings.Web;

namespace ChatTCPServer.Services
{
    /// <summary>
    /// Serializer for serialization from string json to object and object to string json
    /// </summary>
    public class JsonStringSerializer : ISerializer<string>
    {
        private readonly JsonSerializerOptions _options;

        public JsonStringSerializer()
        {
            _options = new JsonSerializerOptions { IgnoreNullValues = true };
        }

        /// <summary>
        /// Deserialize JSON string to T object
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj">JSON string</param>
        /// <returns>T object</returns>
        //public T Deserialize<T>(string obj) where T : class, new() => JsonConvert.DeserializeObject<T>(obj);

        public T Deserialize<T>(string obj) where T : class, new() => System.Text.Json.JsonSerializer.Deserialize<T>(obj, _options);

        /// <summary>
        /// Serialize T object to JSON string
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <returns></returns>
        //public string Serialize<T>(T obj) where T : class, new() => JsonConvert.SerializeObject(obj);

        public string Serialize<T>(T obj) where T : class, new() => System.Text.Json.JsonSerializer.Serialize(obj, _options);
    }
}
