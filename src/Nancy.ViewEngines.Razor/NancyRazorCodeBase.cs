namespace Nancy.ViewEngines.Razor
{
    using System;
    using System.IO;

    public class NancyRazorCodeBase
    {
        public static void WriteAttributeTo(TextWriter writer, string name, Tuple<string, int> prefix, Tuple<string, int> suffix, params AttributeValue[] values)
        {
            var attributeValue = NancyPageBuilder.BuildAttribute(name, prefix, suffix, values);
            WriteLiteralTo(writer, attributeValue);
        }

        /// <summary>
        /// Writes the provided <paramref name="value"/> to the provided <paramref name="writer"/>.
        /// </summary>
        /// <param name="writer">The <see cref="TextWriter"/> that should be written to.</param>
        /// <param name="value">The value that should be written.</param>
        public static void WriteTo(TextWriter writer, object value)
        {
            writer.Write(NancyPageBuilder.HtmlEncode(value));
        }

        /// <summary>
        /// Writes the provided <paramref name="value"/>, as a literal, to the provided <paramref name="writer"/>.
        /// </summary>
        /// <param name="writer">The <see cref="TextWriter"/> that should be written to.</param>
        /// <param name="value">The value that should be written as a literal.</param>
        public static void WriteLiteralTo(TextWriter writer, object value)
        {
            writer.Write(value);
        }

        /// <summary>
        /// Writes the provided <paramref name="value"/> to the provided <paramref name="writer"/>.
        /// </summary>
        /// <param name="writer">The <see cref="TextWriter"/> that should be written to.</param>
        /// <param name="value">The <see cref="HelperResult"/> that should be written.</param>
        public static void WriteTo(TextWriter writer, HelperResult value)
        {
            if (value != null)
            {
                value.WriteTo(writer);
            }
        }

        /// <summary>
        /// Writes the provided <paramref name="value"/>, as a literal, to the provided <paramref name="writer"/>.
        /// </summary>
        /// <param name="writer">The <see cref="TextWriter"/> that should be written to.</param>
        /// <param name="value">The <see cref="HelperResult"/> that should be written as a literal.</param>
        public static void WriteLiteralTo(TextWriter writer, HelperResult value)
        {
            if (value != null)
            {
                value.WriteTo(writer);
            }
        }
    }
}