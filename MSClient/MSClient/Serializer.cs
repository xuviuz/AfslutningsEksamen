using System.Text;

namespace MSClient
{
    class Serializer<T>
    {
        /// <summary>
        /// Get decoded data from the message wrapper
        /// </summary>
        /// <param name="wrapper">message wrapper</param>
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
