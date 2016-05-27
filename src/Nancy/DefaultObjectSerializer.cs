namespace Nancy
{
    using Extensions;
    using System;
    using System.IO;
    using System.Reflection;
    using System.Runtime.Serialization;
    using System.Text;

    /// <summary>
    /// Serializes/Deserializes objects for sessions
    /// </summary>
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
                return string.Empty;
            }

            dynamic serializedObject = (sourceObject is string)
                ? sourceObject
                : AddTypeInformation(sourceObject);

            var json = SimpleJson.SerializeObject(serializedObject);
            return Convert.ToBase64String(Encoding.UTF8.GetBytes(json));
        }

        private static dynamic AddTypeInformation(object sourceObject)
        {
            var assemblyQualifiedName = sourceObject.GetType().GetTypeInfo().AssemblyQualifiedName;

            dynamic serializedObject = sourceObject.ToDynamic();
            serializedObject.TypeObject = assemblyQualifiedName;

            return serializedObject;
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

            try
            {
                var inputBytes = Convert.FromBase64String(sourceString);
                var json = Encoding.UTF8.GetString(inputBytes);

                if (!ContainsTypeDescription(json))
                {
                    return SimpleJson.DeserializeObject(json);
                }

                dynamic serializedObject = SimpleJson.DeserializeObject(json);

                return SimpleJson.DeserializeObject(json, Type.GetType(serializedObject.TypeObject));
            }
            catch (FormatException)
            {
                return null;
            }
            catch (SerializationException)
            {
                return null;
            }
            catch (IOException)
            {
                return null;
            }
        }

        private static bool ContainsTypeDescription(string json)
        {
            return json.Contains("TypeObject");
        }
    }
}
