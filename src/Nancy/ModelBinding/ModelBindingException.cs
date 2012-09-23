namespace Nancy.ModelBinding
{
    using System;
    using System.Runtime.Serialization;

    /// <summary>
    /// Represents an exception when attempting to bind to a model
    /// </summary>
    public class ModelBindingException : Exception
    {
        private const string ExceptionMessage = "Unable to bind to type: {0}";
        private const string PropertyMessage = "; Property: {0}";

        /// <summary>
        /// Gets the model property name, which caused the exception
        /// </summary>
        public virtual string PropertyName { get; private set; }

        /// <summary>
        /// Gets the model type, which caused the exception
        /// </summary>
        public virtual Type BoundType { get; private set; }

        /// <summary>
        /// Gets a message with the model type and property name, which caused the exception
        /// </summary>
        public override string Message
        {
            get
            {
                var message = String.Format(ExceptionMessage, BoundType);
                if (PropertyName != null)
                {
                    message += String.Format(PropertyMessage, PropertyName);
                }
                return message;
            }
        }

        /// <summary>
        /// Initializes a new instance of the ModelBindingException class with a specified model type,
        /// property name and the original exception, which caused the problem
        /// </summary>
        /// <param name="boundType">the model type to bind to</param>
        /// <param name="propertyName">the property name, which failed to bind</param>
        /// <param name="innerException">the original exception, thrown while binding the property</param>
        public ModelBindingException(Type boundType, string propertyName = null, Exception innerException = null)
            : base(null, innerException)
        {
            this.PropertyName = propertyName;
            this.BoundType = boundType;
        }

        protected ModelBindingException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}