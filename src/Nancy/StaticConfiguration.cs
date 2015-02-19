namespace Nancy
{
    using System;
    using System.Diagnostics;
    using System.Linq;

    using Nancy.Bootstrapper;
    using Nancy.Diagnostics;

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
            AllowFileStreamUploadAsync = true;
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
        /// Gets or sets a value indicating whether or not to respond with 405 responses
        /// </summary>
        [Description("Disables 405 responses from being sent to the client.")]
        public static bool DisableMethodNotAllowedResponses { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether or not to enable case sensitivity in query, parameters (DynamicDictionary) and model binding. Enable this to conform with RFC3986.
        /// </summary>
        [Description("Enable case sensitivity in query, parameters (DynamicDictionary) and model binding. Enable this to conform with RFC3986.")]
        public static bool CaseSensitive { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether or not to route HEAD requests explicitly.
        /// </summary>
        [Description("Enables explicit HEAD routing and disables the usage of GET routes for HEAD requests.")]
        public static bool EnableHeadRouting { get; set; }

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
                //Get all non-nancy assemblies, and select the custom attributes
                var assembliesInDebug
                    = AppDomainAssemblyTypeScanner.TypesOf<INancyModule>(ScanMode.ExcludeNancy)
                                                  .Select(x => x.Assembly.GetCustomAttributes(typeof(DebuggableAttribute), true))
                                                  .Where(x => x.Length != 0);

                //if there are any, then return the IsJITTrackingEnabled
                //else if the collection is empty it returns false
                return assembliesInDebug.Any(d => ((DebuggableAttribute)d[0]).IsJITTrackingEnabled);
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

        /// <summary>
        /// Gets or sets a value indicating whether or not to disable request stream switching
        /// </summary>
        public static bool? DisableRequestStreamSwitching { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="Nancy.StaticConfiguration"/> allow file stream
        /// upload async due to mono issues before v4.  Uploads of over 80mb would result in extra padded chars to the filestream corrupting the file.
        /// </summary>
        /// <value><c>true</c> if allow file stream upload async; otherwise, <c>false</c>.</value>
        public static bool AllowFileStreamUploadAsync { get; set; }

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
