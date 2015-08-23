namespace Nancy.Configuration
{
    /// <summary>
    /// Defines the functionality for creating a <see cref="INancyEnvironment"/> instance.
    /// </summary>
    public interface INancyEnvironmentFactory : IHideObjectMembers
    {
        /// <summary>
        /// Creates a new <see cref="INancyEnvironment"/> instance.
        /// </summary>
        /// <returns>A <see cref="INancyEnvironment"/> instance.</returns>
        INancyEnvironment CreateEnvironment();
    }
}