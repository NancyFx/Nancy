namespace Nancy
{
    using System;
    using Nancy.Configuration;

    /// <summary>
    /// An exception related to an invalid configuration created within <see cref="INancyEnvironment"/>
    /// </summary>
    public class ConfigurationException : Exception
    {
        /// <summary>
        /// Create an instance of <see cref="ConfigurationException"/>
        /// </summary>
        /// <param name="message">A message to be passed into the exception</param>
        public ConfigurationException(string message)
            : base(message)
        {
            
        }

        /// <summary>
        /// Create an instance of <see cref="ConfigurationException"/>
        /// </summary>
        /// <param name="message">A message to be passed into the exception</param>
        /// <param name = "exception">An inner exception to buble up</param>
        public ConfigurationException(string message, Exception exception)
            : base(message, exception)
        {
            
        }
    }
}
