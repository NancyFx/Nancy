namespace Nancy.Session
{
    /// <summary>
    /// De/Serialisation for cookie objects
    /// </summary>
    public interface ISessionObjectFormatter
    {
        /// <summary>
        /// Serialize an object
        /// </summary>
        /// <typeparam name="T">Type of the object</typeparam>
        /// <param name="sourceObject">Source object</param>
        /// <returns>Serialised object string</returns>
        string Serialize<T>(T sourceObject);

        /// <summary>
        /// Deserialize an object string
        /// </summary>
        /// <typeparam name="T">Type of the object</typeparam>
        /// <param name="sourceString">Source object string</param>
        /// <returns>Deserialized object</returns>
        T Deserialize<T>(string sourceString);
    }
}