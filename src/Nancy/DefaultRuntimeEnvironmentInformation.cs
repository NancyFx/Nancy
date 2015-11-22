namespace Nancy
{
    using System;
    using System.Diagnostics;
    using System.Linq;
    using Nancy.Bootstrapper;

    /// <summary>
    /// Default implementation of the <see cref="IRuntimeEnvironmentInformation"/> interface.
    /// </summary>
    public class DefaultRuntimeEnvironmentInformation : IRuntimeEnvironmentInformation
    {
        private readonly Lazy<bool> isDebug = GetDebugMode();

        /// <summary>
        /// Gets a value indicating if the application is running in debug mode.
        /// </summary>
        /// <returns><see langword="true"/> if the application is running in debug mode, otherwise <see langword="false"/>.</returns>
        public virtual bool IsDebug
        {
            get { return this.isDebug.Value; }
        }

        private static Lazy<bool> GetDebugMode()
        {
            return new Lazy<bool>(() =>
            {
                try
                {
                    var assembliesInDebug = AppDomainAssemblyTypeScanner
                        .TypesOf<INancyModule>(ScanMode.ExcludeNancy)
                        .Select(x => x.Assembly.GetCustomAttributes(typeof(DebuggableAttribute), true))
                        .Where(x => x.Length != 0);

                    return assembliesInDebug.Any(d => ((DebuggableAttribute)d[0]).IsJITTrackingEnabled);
                }
                catch
                {
                    return false;
                }
            });
        }
    }
}
