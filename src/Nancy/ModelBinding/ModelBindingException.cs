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

        public ModelBindingException(Type boundType)
            : base(string.Format(ExceptionMessage, boundType))
        {
        }

        public ModelBindingException(Type boundType, Exception innerException)
            : base(string.Format(ExceptionMessage, boundType), innerException)
        {
        }

        protected ModelBindingException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}