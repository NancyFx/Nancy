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

        public virtual string PropertyName { get; private set; }

        public virtual Type BoundType { get; private set; }

        public override string Message
        {
            get { var message = String.Format(ExceptionMessage, BoundType);
            if (PropertyName != null)
            {
                message += String.Format(PropertyMessage, PropertyName);
            }
                return message;
            }
        }

        public ModelBindingException(Type boundType, string propertyName = null, Exception innerException = null)
            : base(null, innerException)
        {
            PropertyName = propertyName;
            BoundType = boundType;
        }

        protected ModelBindingException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}