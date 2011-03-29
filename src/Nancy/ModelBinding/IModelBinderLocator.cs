namespace Nancy.ModelBinding
{
    using System;

    /// <summary>
    /// Locates model binders for a particular model
    /// </summary>
    public interface IModelBinderLocator
    {
        /// <summary>
        /// Gets a binder for the given type
        /// </summary>
        /// <param name="modelType">Destination type to bind to</param>
        /// <returns>IModelBinder instance or null if none found</returns>
        IBinder GetBinderForType(Type modelType);
    }
}