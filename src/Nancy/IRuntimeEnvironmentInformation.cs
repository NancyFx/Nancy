namespace Nancy
{
    /// <summary>
    /// Defines functionality for getting information about the runtime execution environment.
    /// </summary>
    public interface IRuntimeEnvironmentInformation
    {
        /// <summary>
        /// Gets a value indicating if the application is running in debug mode.
        /// </summary>
        /// <returns><see langword="true"/> if the application is running in debug mode, otherwise <see langword="false"/>.</returns>
        bool IsDebug { get; }
    }
}
