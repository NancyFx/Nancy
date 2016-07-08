namespace Nancy
{
    using System;
    using System.Diagnostics;
    using System.Linq;
    using Nancy.Bootstrapper;
    using Nancy.Diagnostics;

    /// <summary>
    /// Static configurations.
    /// </summary>
    public static class StaticConfiguration
    {
        static StaticConfiguration()
        {
            CaseSensitive = false;
            RequestQueryFormMultipartLimit = 1000;
            AllowFileStreamUploadAsync = true;
        }

        /// <summary>
        /// Gets or sets a value indicating whether or not to enable case sensitivity in query, parameters (DynamicDictionary) and model binding. Enable this to conform with RFC3986.
        /// </summary>
        [Description("Enable case sensitivity in query, parameters (DynamicDictionary) and model binding. Enable this to conform with RFC3986.")]
        public static bool CaseSensitive { get; set; }

        /// <summary>
        /// Gets or sets the limit on the number of query string variables, form fields,
        /// or multipart sections in a request.
        /// </summary>
        public static int RequestQueryFormMultipartLimit { get; set; }

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
    }
}
