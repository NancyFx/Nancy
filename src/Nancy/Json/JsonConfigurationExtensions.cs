namespace Nancy.Json
{
    using System.Collections.Generic;
    using System.Text;
    using Nancy.Configuration;

    /// <summary>
    /// Contains <see cref="JsonConfiguration"/> configuration extensions for <see cref="INancyEnvironment"/>.
    /// </summary>
    public static class JsonConfigurationExtensions
    {
        /// <summary>
        /// Configures JSON serialization.
        /// </summary>
        /// <param name="environment"><see cref="INancyEnvironment"/> that should be configured.</param>
        /// <param name="defaultEncoding">The <see cref="Encoding"/> that should be as a default.</param>
        /// <param name="converters">List of <see cref="JavaScriptConverter"/> that should be used.</param>
        /// <param name="primitiveConverters">List of <see cref="JavaScriptPrimitiveConverter"/> that should be used.</param>
        /// <param name="retainCasing"><see langword="true" /> if C# casing should be retained, otherwise <see langword="false" /> to use camel-casing.</param>
        /// <param name="serializeEnumToString"><see langword="true"/> if enums should be represented as string otherwise <see langword="false"/>.</param>
        /// <param name="excludeNullValues"><see langword="true" /> if the serializer should exclude null values for properties on objects otherwise <see langword="false" />.</param>
        public static void Json(this INancyEnvironment environment, Encoding defaultEncoding = null, IList<JavaScriptConverter> converters = null, IList<JavaScriptPrimitiveConverter> primitiveConverters = null, bool? retainCasing = null, bool? serializeEnumToString = null, bool? excludeNullValues = false)
        {
            environment.AddValue(new JsonConfiguration(
                defaultEncoding ?? JsonConfiguration.Default.DefaultEncoding,
                converters ?? JsonConfiguration.Default.Converters,
                primitiveConverters ?? JsonConfiguration.Default.PrimitiveConverters,
                retainCasing ?? JsonConfiguration.Default.RetainCasing,
                serializeEnumToString ?? JsonConfiguration.Default.SerializeEnumToString,
                excludeNullValues ?? JsonConfiguration.Default.ExcludeNullValues));
        }
    }
}