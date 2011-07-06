namespace Nancy.Bootstrapper
{
    /// <summary>
    /// Provides a hook to execute code during initialisation
    /// </summary>
    public interface IStartup
    {
        /// <summary>
        /// Perform any initialisation tasks
        /// </summary>
        void Initialize();
    }
}