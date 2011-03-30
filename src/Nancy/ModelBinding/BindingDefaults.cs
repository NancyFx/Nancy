namespace Nancy.ModelBinding
{
    using System.Collections.Generic;

    using Nancy.ModelBinding.DefaultBodyDeserializers;
    using Nancy.ModelBinding.DefaultConverters;

    /// <summary>
    /// Provides default binding converters/deserializers
    /// The defaults have less precedence than any user supplied ones 
    /// </summary>
    public class BindingDefaults
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="T:System.Object"/> class.
        /// </summary>
        public BindingDefaults()
        {
            this.defaultTypeConverters = new ITypeConverter[]
                {
                    new CollectionConverter(),
                    new FallbackConverter(),
                };

            this.defaultBodyDeserializers = new IBodyDeserializer[]
                {
                    new JsonBodyDeserializer(),
                };
        }

        private readonly IEnumerable<ITypeConverter> defaultTypeConverters;

        private readonly IEnumerable<IBodyDeserializer> defaultBodyDeserializers;

        /// <summary>
        /// Gets the default type converters
        /// </summary>
        public virtual IEnumerable<ITypeConverter> DefaultTypeConverters
        {
            get
            {
                return this.defaultTypeConverters;
            }
        }

        /// <summary>
        /// Gets the default type converters
        /// </summary>
        public virtual IEnumerable<IBodyDeserializer> DefaultBodyDeserializers
        {
            get
            {
                return this.defaultBodyDeserializers;
            }
        }
    }
}