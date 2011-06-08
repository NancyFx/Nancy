namespace Nancy.ViewEngines
{
    using System;

    /// <summary>
    /// Defines the functionality of a Nancy view cache.
    /// </summary>
    public interface IViewCache
    {
        /// <summary>
        /// Gets or adds a view from the cache.
        /// </summary>
        /// <typeparam name="TCompiledView">The type of the cached view instance.</typeparam>
        /// <param name="viewLocationResult">A <see cref="ViewLocationResult"/> instance that describes the view that is being added or retrieved from the cache.</param>
        /// <param name="valueFactory">A function that produces the value that should be added to the cache in case it does not already exist.</param>
        /// <returns>An instance of the type specified by the <typeparamref name="TCompiledView"/> type.</returns>
        TCompiledView GetOrAdd<TCompiledView>(ViewLocationResult viewLocationResult, Func<ViewLocationResult, TCompiledView> valueFactory);
    }
}