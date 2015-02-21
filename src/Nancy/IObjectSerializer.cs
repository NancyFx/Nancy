namespace Nancy
{
    /// <summary>
    /// De/Serialization for cookie objects
    /// </summary>
    public interface IObjectSerializer
    {
        /// <summary>
        /// Serialize an object
        /// </summary>
        /// <param name="sourceObject">Source object</param>
        /// <returns>Serialized object string</returns>
        string Serialize(object sourceObject);

        /// <summary>
        /// Deserialize an object string
        /// </summary>
        /// <param name="sourceString">Source object string</param>
        /// <returns>Deserialized object</returns>
        object Deserialize(string sourceString);
    }
}