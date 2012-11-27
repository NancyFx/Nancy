namespace Nancy.Cache
{
    /// <summary>
    /// An interface for defining a generic object cache
    /// </summary>
    public interface ICacheStore
    {
        /// <summary>
        /// Try to load an object back out of the cache by id
        /// </summary>
        /// <typeparam name="T">The type to load</typeparam>
        /// <param name="id">The cache id</param>
        /// <param name="obj">The object that is loaded or default(T)</param>
        /// <returns>Returns whether the oad was successful</returns>
        bool TryLoad<T>(string id, out T obj);
        
        /// <summary>
        /// Store an object into the cache
        /// </summary>
        /// <param name="id">The id to store it under</param>
        /// <param name="obj">The object to store</param>
        void Store(string id, object obj);

        /// <summary>
        /// Remove an object from the cache by id
        /// </summary>
        /// <param name="id">The id of the object to remove</param>
        void Remove(string id);
    }
}
