namespace Nancy
{
    /// <summary>
    /// De/Serialisation for cookie objects
    /// </summary>
    public interface IObjectSerializer
    {
        /// <summary>
        /// Serialize an object
        /// </summary>
        /// <param name="sourceObject">Source object</param>
        /// <returns>Serialised object string</returns>
        string Serialize(object sourceObject);

        /// <summary>
        /// Deserialize an object string
        /// </summary>
        /// <param name="sourceString">Source object string</param>
        /// <returns>Deserialized object</returns>
        object Deserialize(string sourceString);
    }
}