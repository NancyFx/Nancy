namespace Nancy.Json
{
    using System.Collections.Generic;
    using System.Text;
    using Nancy.Configuration;
    using Nancy.Diagnostics;

    /// <summary>
    /// Contains <see cref="DiagnosticsConfiguration"/> configuration extensions for <see cref="INancyEnvironment"/>.
    /// </summary>
    public static class JsonConfigurationExtensions
    {

        /// <summary>
        /// Configures JSON serialization.
        /// </summary>
        /// <param name="environment"><see cref="INancyEnvironment"/> that should be configured.</param>
        /// <param name="iso8601DateFormat"><see langword="true" /> if ISO-860 date formats should be used, otherwise <see langword="false" />.</param>
        /// <param name="maxJsonLength">Max length of JSON output.</param>
        /// <param name="maxRecursions">Maximum number of recursions.</param>
        /// <param name="defaultEncoding">The <see cref="Encoding"/> that should be as a default.</param>
        /// <param name="converters">List of <see cref="JavaScriptConverter"/> that should be used.</param>
        /// <param name="primitiveConverters">List of <see cref="JavaScriptPrimitiveConverter"/> that should be used.</param>
        /// <param name="retainCasing"><see langword="true" /> if C# casing should be retained, otherwise <see langword="false" /> to use camel-casing.</param>
        public static void Json(this INancyEnvironment environment, bool? iso8601DateFormat = null, int? maxJsonLength = null, int? maxRecursions = null, Encoding defaultEncoding = null, IList<JavaScriptConverter> converters = null, IList<JavaScriptPrimitiveConverter> primitiveConverters = null, bool? retainCasing = null)
        {
            environment.AddValue(new JsonConfiguration(
                iso8601DateFormat ?? JsonConfiguration.Default.ISO8601DateFormat,
                maxJsonLength ?? JsonConfiguration.Default.MaxJsonLength,
                maxRecursions ?? JsonConfiguration.Default.MaxRecursions,
                defaultEncoding,
                converters,
                primitiveConverters,
                retainCasing ?? JsonConfiguration.Default.RetainCasing));
        }
    }
}