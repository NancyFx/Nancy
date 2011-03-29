namespace Nancy.ModelBinding
{
    using System;

    /// <summary>
    /// Provides fallback for binding an incoming request, via the context, to a model type
    /// </summary>
    public interface IBinder
    {
        /// <summary>
        /// Bind to the given model type
        /// </summary>
        /// <param name="context">Current context</param>
        /// <param name="modelType">Model type to bind to</param>
        /// <returns>Bound model</returns>
        object Bind(NancyContext context, Type modelType);
    }
}