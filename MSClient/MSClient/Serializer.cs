using System.Text;

namespace MSClient
{
    /// <summary>
    /// Represents class for encoding/decoding data
    /// </summary>
    /// <typeparam name="T"></typeparam>
    class Serializer<T>
    {
        /// <summary>
        /// Get decoded data from encoded byte array
        /// </summary>
        /// <param name="wrapper">encoded byte array</param>
        /// <returns>decoded data</returns>
        public static T GetData(byte[] tempData)
        {
            T data = default(T);
            string json = Encoding.UTF8.GetString(tempData);
            data = Newtonsoft.Json.JsonConvert.DeserializeObject<T>(json);

            return data;
        }
    }
}
