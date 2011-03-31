namespace Nancy.ModelBinding
{
    using System;
    using System.Collections.Generic;
    using System.Reflection;

    /// <summary>
    /// Model binding context object
    /// </summary>
    public class BindingContext
    {
        /// <summary>
        /// Current Nancy context
        /// </summary>
        public NancyContext Context { get; set; }

        /// <summary>
        /// Binding destination type
        /// </summary>
        public Type DestinationType { get; set; }

        /// <summary>
        /// The current model object (or null for body deserialization)
        /// </summary>
        public object Model { get; set; }

        /// <summary>
        /// DestinationType properties that are not black listed
        /// </summary>
        public IEnumerable<PropertyInfo> ValidModelProperties { get; set; }

        /// <summary>
        /// The incoming data fields
        /// </summary>
        public IDictionary<string, string> RequestData { get; set; }

        /// <summary>
        /// Available type converters - user converters followed by any defaults
        /// </summary>
        public IEnumerable<ITypeConverter> TypeConverters { get; set; }
    }
}