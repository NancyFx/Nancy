namespace Nancy.Hosting.Self
{
    /// <summary>
    /// The concurrency mode of the host.
    /// </summary>
    public enum ConcurrencyMode
    {
        /// <summary>
        /// Process all incoming requests on multiple threads.
        /// </summary>
        Multiple = 0,
        /// <summary>
        /// Process all incoming requests on the same thread (treating them all sequential).
        /// </summary>
        Single = 1
    }
}
