namespace Nancy.Prototype
{
    using System;

    public class BootstrapperActivationException : Exception
    {
        public BootstrapperActivationException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}
