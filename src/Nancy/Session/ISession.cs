namespace Nancy.Session
{
    using System.Collections.Generic;

    /// <summary>
    /// Defines the interface for a session
    /// </summary>
    public interface ISession : IEnumerable<KeyValuePair<string, object>>
    {
        /// <summary>
        /// The number of session values
        /// </summary>
        /// <returns></returns>
        int Count { get; }

        /// <summary>
        /// Deletes the session and all associated information
        /// </summary>
        void DeleteAll();

        /// <summary>
        /// Deletes the specific key from the session
        /// </summary>
        void Delete(string key);

        /// <summary>
        /// Retrieves the value from the session
        /// </summary>        
        object this[string key] { get; set; }

        bool HasChanged { get; }
    }
}