namespace Nancy.ModelBinding
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// Locates model binders for a particular model
    /// </summary>
    public class ModelBinderLocator : IModelBinderLocator
    {
        /// <summary>
        /// Available model binders
        /// </summary>
        private readonly IEnumerable<IModelBinder> binders;

        /// <summary>
        /// Default model binder to fall back on
        /// </summary>
        private readonly IModelBinder defaultBinder = new DefaultModelBinder(); // TODO: Should we make the "fallback" removable/pluggable?

        /// <summary>
        /// Initializes a new instance of the <see cref="ModelBinderLocator"/> class.
        /// </summary>
        /// <param name="binders">Available model binders</param>
        public ModelBinderLocator(IEnumerable<IModelBinder> binders)
        {
            var defaultBinderType = typeof(DefaultModelBinder);

            this.binders = binders != null
                               ? binders.Where(b => b.GetType() != defaultBinderType)
                               : new IModelBinder[] { };
        }

        /// <summary>
        /// Gets a binder for the given type
        /// </summary>
        /// <param name="modelType">Destination type to bind to</param>
        /// <returns>IModelBinder instance or null if none found</returns>
        public IModelBinder GetBinderForType(Type modelType)
        {
            return this.binders.Where(modelBinder => modelBinder.CanBind(modelType)).FirstOrDefault() ?? this.defaultBinder;
        }
    }
}