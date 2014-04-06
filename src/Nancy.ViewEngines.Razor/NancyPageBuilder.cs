namespace Nancy.ViewEngines.Razor
{
    using System;
    using System.Globalization;
    using System.Text;

    using Nancy.Helpers;

    internal class NancyPageBuilder
    {
        public static string BuildAttribute(string name, Tuple<string, int> prefix, Tuple<string, int> suffix,
                                            params AttributeValue[] values)
        {
            var writtenAttribute = false;
            var attributeBuilder = new StringBuilder(prefix.Item1);

            foreach (var value in values)
            {
                if (ShouldWriteValue(value.Value.Item1))
                {
                    var stringValue = GetStringValue(value);
                    var valuePrefix = value.Prefix.Item1;

                    if (!string.IsNullOrEmpty(valuePrefix))
                    {
                        attributeBuilder.Append(valuePrefix);
                    }

                    attributeBuilder.Append(stringValue);
                    writtenAttribute = true;
                }
            }

            attributeBuilder.Append(suffix.Item1);

            var renderAttribute = writtenAttribute || values.Length == 0;

            if (renderAttribute)
            {
                return attributeBuilder.ToString();
            }

            return string.Empty;
        }

        /// <summary>
        /// Html encodes an object if required
        /// </summary>
        /// <param name="value">Object to potentially encode</param>
        /// <returns>String representation, encoded if necessary</returns>
        public static string HtmlEncode(object value)
        {
            if (value == null)
            {
                return null;
            }

            var str = value as IHtmlString;

            return str != null ? str.ToHtmlString() : HttpUtility.HtmlEncode(Convert.ToString(value, CultureInfo.CurrentCulture));
        }

        private static string GetStringValue(AttributeValue value)
        {
            if (value.IsLiteral)
            {
                return (string)value.Value.Item1;
            }

            if (value.Value.Item1 is IHtmlString)
            {
                return ((IHtmlString)value.Value.Item1).ToHtmlString();
            }

            if (value.Value.Item1 is DynamicDictionaryValue)
            {
                var dynamicValue = (DynamicDictionaryValue)value.Value.Item1;
                return dynamicValue.HasValue ? dynamicValue.Value.ToString() : string.Empty;
            }

            return value.Value.Item1.ToString();
        }

        private static bool ShouldWriteValue(object value)
        {
            if (value == null)
            {
                return false;
            }

            if (value is bool)
            {
                var boolValue = (bool)value;

                return boolValue;
            }

            return true;
        }
    }
}