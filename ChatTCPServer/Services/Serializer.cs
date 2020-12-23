using Newtonsoft.Json;
using ServerBusinessLogic.Interfaces;

namespace ChatTCPServer.Services
{
    /// <summary>
    /// Serializer for serialization from string and to string
    /// </summary>
    public class Serializer : ISerializer
    {
        /// <summary>
        /// Deserialize JSON string to T object
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj">JSON string</param>
        /// <returns>T object</returns>
        public T Deserialize<T>(string obj) where T : class, new() => JsonConvert.DeserializeObject<T>(obj);

        /// <summary>
        /// Serialize T object to JSON string
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <returns></returns>
        public string Serialize<T>(T obj) where T : class, new() => JsonConvert.SerializeObject(obj);
    }
}
