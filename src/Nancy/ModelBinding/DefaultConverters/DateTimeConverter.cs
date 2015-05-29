namespace Nancy.ModelBinding.DefaultConverters
{
    using System;
    using System.Globalization;

    /// <summary>
    /// Converter for datetime types
    /// </summary>
    public class DateTimeConverter : ITypeConverter
    {
        /// <summary>
        /// Used to ensure Converter is always called before FallbackConverter
        /// </summary>
        public int Order { get { return 0; } }

        /// <summary>
        /// Whether the converter can convert to the destination type
        /// </summary>
        /// <param name="destinationType">Destination type</param>
        /// <param name="context">The current binding context</param>
        /// <returns>True if conversion supported, false otherwise</returns>
        public bool CanConvertTo(Type destinationType, BindingContext context)
        {
            return destinationType == typeof(DateTime);
        }

        /// <summary>
        /// Convert the string representation to the destination type
        /// </summary>
        /// <param name="input">Input string</param>
        /// <param name="destinationType">Destination type</param>
        /// <param name="context">Current context</param>
        /// <returns>Converted object of the destination type</returns>
        public object Convert(string input, Type destinationType, BindingContext context)
        {
            if (string.IsNullOrEmpty(input))
            {
                return null;
            }

            DateTime result;
            var culture = context.Context.Culture ?? CultureInfo.CurrentCulture;

            if (DateTime.TryParse(input, culture.DateTimeFormat, DateTimeStyles.None, out result))
            {
                return result;
            }

            throw new FormatException("The string was not recognized as a valid DateTime. There is an unknown word starting at index 0.");
        }
    }
}
