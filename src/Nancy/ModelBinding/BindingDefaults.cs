namespace Nancy.ModelBinding
{
    using System.Collections.Generic;
    using Nancy.Configuration;
    using Nancy.ModelBinding.DefaultBodyDeserializers;
    using Nancy.ModelBinding.DefaultConverters;

    /// <summary>
    /// Provides default binding converters/deserializers
    /// The defaults have less precedence than any user supplied ones
    /// </summary>
    public class BindingDefaults
    {
        private readonly IEnumerable<ITypeConverter> defaultTypeConverters;
        private readonly IEnumerable<IBodyDeserializer> defaultBodyDeserializers;

        /// <summary>
        /// Initializes a new instance of the <see cref="BindingDefaults"/> class,
        /// with the provided <see cref="INancyEnvironment"/>.
        /// </summary>
        /// <param name="environment">An <see cref="INancyEnvironment"/> instance.</param>
        public BindingDefaults(INancyEnvironment environment)
        {
            // Ordering is important - for now we will new just these up
            // as the binding defaults class itself is replaceable if necessary,
            // and none of defaults have any dependencies.
            this.defaultTypeConverters = new ITypeConverter[]
            {
                new CollectionConverter(),
                new FallbackConverter(),
            };

            this.defaultBodyDeserializers = new IBodyDeserializer[]
            {
                new JsonBodyDeserializer(environment),
                new XmlBodyDeserializer()
            };
        }

        /// <summary>
        /// Gets the default type converters
        /// </summary>
        /// <value>An <see cref="IEnumerable{T}"/> of <see cref="ITypeConverter"/> instances.</value>
        public virtual IEnumerable<ITypeConverter> DefaultTypeConverters
        {
            get { return this.defaultTypeConverters; }
        }

        /// <summary>
        /// Gets the default type converters
        /// </summary>
        /// <value>An <see cref="IEnumerable{T}"/> of <see cref="IBodyDeserializer"/> instances.</value>
        public virtual IEnumerable<IBodyDeserializer> DefaultBodyDeserializers
        {
            get { return this.defaultBodyDeserializers; }
        }
    }
}
