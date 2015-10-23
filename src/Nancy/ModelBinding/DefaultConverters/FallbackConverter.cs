namespace Nancy.ModelBinding.DefaultConverters
{
    using System;
    using System.ComponentModel;

    /// <summary>
    /// A fallback converter that uses TypeDescriptor.GetConverter to try
    /// and convert the value.
    /// </summary>
    public class FallbackConverter : ITypeConverter
    {
        /// <summary>
        /// Whether the converter can convert to the destination type
        /// </summary>
        /// <param name="destinationType">Destination type</param>
        /// <param name="context">The current binding context</param>
        /// <returns>True if conversion supported, false otherwise</returns>
        public bool CanConvertTo(Type destinationType, BindingContext context)
        {
            return true;
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
            try
            {
                if (destinationType.IsEnum)
                {
                    return Enum.Parse(destinationType, input);
                }

                return System.Convert.ChangeType(input, destinationType);
            }
            catch (FormatException)
            {
                if (destinationType == typeof(bool) && "on".Equals(input, StringComparison.OrdinalIgnoreCase))
                {
                    return true;
                }

                throw;
            }
        }
    }
}