namespace Nancy.Validation.FluentValidation
{
    using System.Collections.Generic;
    using System.Linq;

    using global::FluentValidation.Validators;

    /// <summary>
    /// Default implementation of the <see cref="IFluentAdapterFactory"/> interface.
    /// </summary>
    public class DefaultFluentAdapterFactory : IFluentAdapterFactory
    {
        private readonly IEnumerable<IFluentAdapter> adapters;

        /// <summary>
        /// Initializes a new instance of the <see cref="DefaultFluentAdapterFactory"/> class.
        /// </summary>
        public DefaultFluentAdapterFactory(IEnumerable<IFluentAdapter> adapters)
        {
            this.adapters = adapters;
        }

        /// <summary>
        /// Creates a <see cref="IFluentAdapter"/> instance based on the provided <paramref name="propertyValidator"/>.
        /// </summary>
        /// <param name="propertyValidator">The <see cref="IPropertyValidator"/> for which the adapter should be created.</param>
        /// <returns>An <see cref="IFluentAdapter"/> instance.</returns>
        public IFluentAdapter Create(IPropertyValidator propertyValidator)
        {
            var adapter =
                this.adapters.SingleOrDefault(x => x.CanHandle(propertyValidator));

            return adapter ?? new FallbackAdapter();
        }
    }
}