namespace Nancy.ModelBinding
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// Locates model binders for a particular model
    /// </summary>
    public class DefaultModelBinderLocator : IModelBinderLocator
    {
        /// <summary>
        /// Available model binders
        /// </summary>
        private readonly IEnumerable<IModelBinder> binders;

        /// <summary>
        /// Default model binder to fall back on
        /// </summary>
        private readonly IBinder fallbackBinder;

        /// <summary>
        /// Initializes a new instance of the <see cref="DefaultModelBinderLocator"/> class.
        /// </summary>
        /// <param name="binders">Available model binders</param>
        /// <param name="fallbackBinder">Fallback binder</param>
        public DefaultModelBinderLocator(IEnumerable<IModelBinder> binders, IBinder fallbackBinder)
        {
            this.fallbackBinder = fallbackBinder;

            this.binders = binders;
        }

        /// <summary>
        /// Gets a binder for the given type
        /// </summary>
        /// <param name="modelType">Destination type to bind to</param>
        /// <returns>IModelBinder instance or null if none found</returns>
        public IBinder GetBinderForType(Type modelType)
        {
            return this.binders.Where(modelBinder => modelBinder.CanBind(modelType)).FirstOrDefault() ?? this.fallbackBinder;
        }
    }
}