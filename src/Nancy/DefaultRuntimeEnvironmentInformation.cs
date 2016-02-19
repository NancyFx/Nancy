namespace Nancy
{
    using System;
    using System.Diagnostics;
    using System.Linq;
    using System.Reflection;

    /// <summary>
    /// Default implementation of the <see cref="IRuntimeEnvironmentInformation"/> interface.
    /// </summary>
    public class DefaultRuntimeEnvironmentInformation : IRuntimeEnvironmentInformation
    {
        private readonly Lazy<bool> isDebug;

        /// <summary>
        /// Initializes a new instance of the <see cref="DefaultRuntimeEnvironmentInformation"/> class.
        /// </summary>
        /// <param name="typeCatalog">An <see cref="ITypeCatalog"/> instance.</param>
        public DefaultRuntimeEnvironmentInformation(ITypeCatalog typeCatalog)
        {
            this.isDebug = new Lazy<bool>(() => GetDebugMode(typeCatalog));
        }

        /// <summary>
        /// Gets a value indicating if the application is running in debug mode.
        /// </summary>
        /// <returns><see langword="true"/> if the application is running in debug mode, otherwise <see langword="false"/>.</returns>
        public virtual bool IsDebug
        {
            get { return this.isDebug.Value; }
        }

        private static bool GetDebugMode(ITypeCatalog typeCatalog)
        {
            try
            {
                return Debugger.IsAttached;
            }
            catch
            {
                return false;
            }
        }
    }
}
