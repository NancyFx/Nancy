namespace Nancy.ModelBinding
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Model binding context object
    /// </summary>
    public class BindingContext
    {
        /// <summary>
        /// The binding configuration
        /// </summary>
        public BindingConfig Configuration { get; set; }

        /// <summary>
        /// Current Nancy context
        /// </summary>
        public NancyContext Context { get; set; }

        /// <summary>
        /// Binding destination type
        /// </summary>
        public Type DestinationType { get; set; }

        /// <summary>
        /// The generic type of a collection is only used when DestinationType is a enumerable.
        /// </summary>
        public Type GenericType { get; set; }

        /// <summary>
        /// The current model object (or null for body deserialization)
        /// </summary>
        public object Model { get; set; }

        /// <summary>
        /// DestinationType properties that are not black listed
        /// </summary>
        public IEnumerable<BindingMemberInfo> ValidModelBindingMembers { get; set; }

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