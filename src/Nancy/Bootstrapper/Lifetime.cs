namespace Nancy.Bootstrapper
{
    /// <summary>
    /// Represents the lifetime of a container registration
    /// </summary>
    public enum Lifetime
    {
        /// <summary>
        /// Transient lifetime - each request to the container for
        /// the type will result in a new version being returned.
        /// </summary>
        Transient,

        /// <summary>
        /// Singleton - each request to the container for the type
        /// will result in the same instance being returned.
        /// </summary>
        Singleton,

        /// <summary>
        /// PerRequest - within the context of each request each request
        /// for the type will result in the same instance being returned.
        /// Different requests will have different versions.
        /// </summary>
        PerRequest
    }
}