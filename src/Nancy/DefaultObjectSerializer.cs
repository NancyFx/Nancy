namespace Nancy
{
    using System;
    using System.IO;
    using System.Runtime.Serialization.Formatters.Binary;

    public class DefaultObjectSerializer : IObjectSerializer
    {
        /// <summary>
        /// Serialize an object
        /// </summary>
        /// <param name="sourceObject">Source object</param>
        /// <returns>Serialised object string</returns>
        public string Serialize(object sourceObject)
        {
            if (sourceObject == null)
            {
                return String.Empty;
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
        /// <param name="sourceString">Source object string</param>
        /// <returns>Deserialized object</returns>
        public object Deserialize(string sourceString)
        {
            if (string.IsNullOrEmpty(sourceString))
            {
                return null;
            }

            var inputBytes = Convert.FromBase64String(sourceString);

            var formatter = new BinaryFormatter();

            using (var inputStream = new MemoryStream(inputBytes, false))
            {
                return formatter.Deserialize(inputStream);
            }
        }
    }
}