namespace Nancy.ModelBinding
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.Serialization;

    /// <summary>
    /// Represents an exception when attempting to bind to a model
    /// </summary>
    public class ModelBindingException : Exception
    {
        private const string ExceptionMessage = "Unable to bind to type: {0}";

        /// <summary>
        /// Gets all failures
        /// </summary>
        public virtual IEnumerable<PropertyBindingException> PropertyBindingExceptions { get; private set; }

        /// <summary>
        /// Gets the model type, which caused the exception
        /// </summary>
        public virtual Type BoundType { get; private set; }

        /// <summary>
        /// Initializes a new instance of the ModelBindingException class with a specified model type,
        /// property name and the original exception, which caused the problem
        /// </summary>
        /// <param name="boundType">the model type to bind to</param>
        /// <param name="propertyBindingExceptions">the original exceptions, thrown while binding the property</param>
        public ModelBindingException(Type boundType, IEnumerable<PropertyBindingException> propertyBindingExceptions = null)
            : base(String.Format(ExceptionMessage, boundType))
        {
            if (boundType == null)
            {
                throw new ArgumentNullException("boundType");
            }
            this.PropertyBindingExceptions = propertyBindingExceptions ?? new List<PropertyBindingException>();
            this.BoundType = boundType;
        }

        protected ModelBindingException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}