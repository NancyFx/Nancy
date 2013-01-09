namespace Nancy.ModelBinding.DefaultConverters
{
    using System;

    /// <summary>
    /// Converter for datetime types
    /// </summary>
    public class DateTimeConverter : ITypeConverter
    {
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

            return System.Convert.ChangeType(input, destinationType, context.Context.Culture);
        }
    }
}
