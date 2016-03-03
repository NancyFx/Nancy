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
        /// <param name="maxJsonLength">Max length of JSON output.</param>
        /// <param name="defaultEncoding">The <see cref="Encoding"/> that should be as a default.</param>
        /// <param name="converters">List of <see cref="JavaScriptConverter"/> that should be used.</param>
        /// <param name="primitiveConverters">List of <see cref="JavaScriptPrimitiveConverter"/> that should be used.</param>
        /// <param name="retainCasing"><see langword="true" /> if C# casing should be retained, otherwise <see langword="false" /> to use camel-casing.</param>
        public static void Json(this INancyEnvironment environment, int? maxJsonLength = null, Encoding defaultEncoding = null, IList<JavaScriptConverter> converters = null, IList<JavaScriptPrimitiveConverter> primitiveConverters = null, bool? retainCasing = null)
        {
            environment.AddValue(new JsonConfiguration(
                defaultEncoding ?? JsonConfiguration.Default.DefaultEncoding,
                converters ?? JsonConfiguration.Default.Converters,
                primitiveConverters ?? JsonConfiguration.Default.PrimitiveConverters,
                retainCasing ?? JsonConfiguration.Default.RetainCasing));
        }
    }
}