namespace Nancy.Diagnostics
{
    /// <summary>
    /// Defines the functionality for creating an <see cref="IRequestTrace"/> instance.
    /// </summary>
    public interface IRequestTraceFactory
    {
        /// <summary>
        /// Creates an <see cref="IRequestTrace"/> instance.
        /// </summary>
        /// <param name="request">A <see cref="Request"/> instance.</param>
        /// <returns>An <see cref="IRequestTrace"/> instance.</returns>
        IRequestTrace Create(Request request);
    }
}