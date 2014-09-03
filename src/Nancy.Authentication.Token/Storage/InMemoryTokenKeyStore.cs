namespace Nancy.Authentication.Token.Storage
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// In in memory implementation of <see cref="ITokenKeyStore"/>. Useful for testing or scenarios
    /// where encryption keys do not need to persist across application restarts (due to updates, app pool 
    /// expiration, etc.)
    /// </summary>
    public class InMemoryTokenKeyStore : ITokenKeyStore
    {
        private IDictionary<DateTime, byte[]> keys;

        /// <summary>
        /// Retrieves encryption keys
        /// </summary>
        /// <returns>Keys</returns>
        public IDictionary<DateTime, byte[]> Retrieve()
        {
            return new Dictionary<DateTime, byte[]>(this.keys ?? new Dictionary<DateTime, byte[]>());
        }

        /// <summary>
        /// Stores encryption keys
        /// </summary>
        /// <param name="keys">Keys</param>
        public void Store(IDictionary<DateTime, byte[]> keys)
        {
            this.keys = new Dictionary<DateTime, byte[]>(keys);
        }

        /// <summary>
        /// Purges encryption keys
        /// </summary>
        public void Purge()
        {
            this.keys = new Dictionary<DateTime, byte[]>();
        }
    }
}