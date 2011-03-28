namespace Nancy.ModelBinding
{
    using System;

    /// <summary>
    /// Provides a way to bind an incoming request, via the context, to a model type
    /// </summary>
    public interface IModelBinder
    {
        /// <summary>
        /// Whether the binder can bind to the given model type
        /// </summary>
        /// <param name="modelType">Required model type</param>
        /// <returns>True if binding is possible, false otherwise</returns>
        bool CanBind(Type modelType);

        /// <summary>
        /// Bind to the given model type
        /// </summary>
        /// <param name="context">Current context</param>
        /// <param name="modelType">Model type to bind to</param>
        /// <returns>Bound model</returns>
        object Bind(NancyContext context, Type modelType);
    }
}