namespace Nancy.Session
{
    using System;
    using System.IO;
    using System.Runtime.Serialization.Formatters.Binary;

    public class DefaultSessionObjectFormatter : ISessionObjectFormatter
    {
        /// <summary>
        /// Serialize an object
        /// </summary>
        /// <typeparam name="T">Type of the object</typeparam>
        /// <param name="sourceObject">Source object</param>
        /// <returns>Serialised object string</returns>
        public string Serialize<T>(T sourceObject)
        {
            if (typeof(T) == typeof(string))
            {
                return sourceObject as string;
            }

            var formatter = new BinaryFormatter();

            using (var outputStream = new MemoryStream())
            {
                formatter.Serialize(outputStream, sourceObject);

                var outputBytes = outputStream.GetBuffer();

                return Convert.ToBase64String(outputStream.GetBuffer());
            }
        }

        /// <summary>
        /// Deserialize an object string
        /// </summary>
        /// <typeparam name="T">Type of the object</typeparam>
        /// <param name="sourceString">Source object string</param>
        /// <returns>Deserialized object</returns>
        public T Deserialize<T>(string sourceString)
        {
            if (typeof(T) == typeof(string))
            {
                return (T)(object)sourceString;
            }

            var inputBytes = Convert.FromBase64String(sourceString);

            var formatter = new BinaryFormatter();

            using (var inputStream = new MemoryStream(inputBytes, false))
            {
                return (T)formatter.Deserialize(inputStream);
            }
        }
    }
}