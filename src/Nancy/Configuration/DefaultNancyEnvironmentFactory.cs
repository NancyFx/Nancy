namespace Nancy.Configuration
{
    /// <summary>
    /// Default implementation of the <see cref="INancyEnvironmentFactory"/> interface.
    /// </summary>
    /// <remarks>Creates instances of the <see cref="DefaultNancyEnvironment"/> type.</remarks>
    public class DefaultNancyEnvironmentFactory : INancyEnvironmentFactory
    {
        /// <summary>
        /// Creates a new <see cref="INancyEnvironment"/> instance.
        /// </summary>
        /// <returns>A <see cref="INancyEnvironment"/> instance.</returns>
        public INancyEnvironment CreateEnvironment()
        {
            return new DefaultNancyEnvironment();
        }
    }
}