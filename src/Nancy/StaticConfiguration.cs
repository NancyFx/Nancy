namespace Nancy
{
    using System;
    using System.Diagnostics;
    using System.Linq;
    using Diagnostics;
    using Nancy.Bootstrapper;

    public static class StaticConfiguration
    {
        private static bool? isRunningDebug;
        private static bool? disableCaches;

        private static bool? disableErrorTraces;

        static StaticConfiguration()
        {
            disableErrorTraces = !(disableCaches = IsRunningDebug);
            CaseSensitive = false;
        }

        /// <summary>
        /// Gets or sets a value indicating whether Nancy should disable caching
        /// </summary>
        [Description("Determins if Nancy should disable the internal caches. This will have an impact on performance and should not be used in production.")]
        public static bool DisableCaches
        {
            get
            {
                return disableCaches ?? (bool)(disableCaches = IsRunningDebug);
            }
            set
            {
                disableCaches = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether or not to disable traces in error messages
        /// </summary>
        [Description("Disables trace output in the default 500 error pages.")]
        public static bool DisableErrorTraces
        {
            get
            {
                return disableErrorTraces ?? (bool)(disableErrorTraces = IsRunningDebug);
            }
            set
            {
                disableErrorTraces = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether or not to enable case sensitivity in query, parameters (DynamicDictionary) and model binding. Enable this to conform with RFC3986.
        /// </summary>
        [Description("Enable case sensitivity in query, parameters (DynamicDictionary) and model binding. Enable this to conform with RFC3986.")]
        public static bool CaseSensitive { get; set; }

        /// <summary>
        /// Gets a value indicating whether we are running in debug mode or not.
        /// Checks the entry assembly to see whether it has been built in debug mode.
        /// If anything goes wrong it returns false.
        /// </summary>
        public static bool IsRunningDebug
        {
            get
            {
                return isRunningDebug ?? (bool)(isRunningDebug = GetDebugMode());
            }
        }

        private static bool GetDebugMode()
        {
            try
            {
                var assembly = AppDomainAssemblyTypeScanner.TypesOf<NancyModule>(true).FirstOrDefault().Assembly;

                var attributes = assembly.GetCustomAttributes(typeof(DebuggableAttribute), true);

                if (attributes.Length == 0)
                {
                    return false;
                }

                var d = (DebuggableAttribute)attributes[0];

                return d.IsJITTrackingEnabled;
            }
            catch (Exception)
            {
                // Evil catch all - don't want to blow up trying to detect debug mode!
                return false;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether or not to enable request tracing
        /// </summary>
        [Description("Enable request tracing.")]
        public static bool EnableRequestTracing { get; set; }
    }
}
