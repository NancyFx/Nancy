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
            RequestQueryFormMultipartLimit = 1000;
        }

        /// <summary>
        /// Gets or sets a value indicating whether Nancy should disable caching
        /// </summary>
        [Obsolete("DisableCaches is now obsolete, please see the StaticConfiguration.Caching properties for more finely grained control", true)]
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

        /// <summary>
        /// Gets or sets the limit on the number of query string variables, form fields,
        /// or multipart sections in a request.
        /// </summary>
        public static int RequestQueryFormMultipartLimit { get; set; }

        private static bool GetDebugMode()
        {
            try
            {
                var assembly = AppDomainAssemblyTypeScanner.TypesOf<INancyModule>(true).FirstOrDefault().Assembly;

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

        public static class Caching
        {
            private static bool? enableRuntimeViewDiscovery;

            private static bool? enableRuntimeViewUpdates;

            /// <summary>
            /// Gets or sets a value indicating whether or not to enable runtime view discovery
            /// Defaults to True in debug mode and False in release mode
            /// </summary>
            [Description("Enable runtime discovery of new views.")]
            public static bool EnableRuntimeViewDiscovery
            {
                get
                {
                    return enableRuntimeViewDiscovery ?? (bool)(enableRuntimeViewDiscovery = IsRunningDebug);
                }
                set
                {
                    enableRuntimeViewDiscovery = value;
                }
            }

            /// <summary>
            /// Gets or sets a value indicating whether or not to allow runtime changes of views
            /// Defaults to True in debug mode and False in release mode
            /// </summary>
            [Description("Enable runtime updating of view templates.")]
            public static bool EnableRuntimeViewUpdates
            {
                get
                {
                    return enableRuntimeViewUpdates ?? (bool)(enableRuntimeViewUpdates = IsRunningDebug);
                }
                set
                {
                    enableRuntimeViewUpdates = value;
                }
            }
        }
    }
}
