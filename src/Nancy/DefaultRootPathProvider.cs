namespace Nancy
{
#if DNX451
    using Microsoft.Framework.Runtime;
    using Microsoft.Framework.Runtime.Infrastructure;
#endif

    /// <summary>
    /// Default implementation of <see cref="IRootPathProvider"/>.
    /// </summary>
    public class DefaultRootPathProvider : IRootPathProvider
    {
        private readonly IApplicationEnvironment env;

        public DefaultRootPathProvider()
        {
#if DNX451
            this.env = CallContextServiceLocator.Locator.ServiceProvider.GetService(typeof(IApplicationEnvironment)) as IApplicationEnvironment;
#endif

        }
        /// <summary>
        /// Returns the root folder path of the current Nancy application.
        /// </summary>
        /// <returns>A <see cref="string"/> containing the path of the root folder.</returns>
        public string GetRootPath()
        {
#if DNX451
            return env.ApplicationBasePath;
#else
            return AppDomain.CurrentDomain.BaseDirectory;
#endif
        }
    }
}