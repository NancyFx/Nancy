namespace Nancy.ModelBinding.DefaultConverters
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.ComponentModel;

    /// <summary>
    /// Converter for handling enumerable types
    /// </summary>
    public class CollectionConverter : ITypeConverter
    {
        /// <summary>
        /// Whether the converter can convert to the destination type
        /// </summary>
        /// <param name="destinationType">Destination type</param>
        /// <returns>True if conversion supported, false otherwise</returns>
        public bool CanConvertTo(Type destinationType)
        {
            return typeof(IEnumerable).IsAssignableFrom(destinationType);
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
            var collection = new List<string>();

            var converter = TypeDescriptor.GetConverter(destinationType);

            if (converter == null || !converter.CanConvertFrom(typeof(IEnumerable)))
            {
                return null;
            }

            throw new NotImplementedException();
        }
    }
}