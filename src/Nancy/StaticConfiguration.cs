namespace Nancy
{
    using System;
    using System.Diagnostics;
    using System.Linq;

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
        public static bool CaseSensitive { get; set; }

        /// <summary>
        /// Gets a value indicating whether we are running in debug mode or not.
        /// Checks the entry assembly to see whether it has been built in debug mode.
        /// If anything goes wrong it returns false.
        /// </summary>
        private static bool IsRunningDebug
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
    }
}
