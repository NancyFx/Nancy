namespace Nancy.ModelBinding.DefaultConverters
{
    using System;
    using System.Linq;
    using System.Reflection;

    using Nancy.Extensions;

    /// <summary>
    /// Converter for handling enumerable types
    /// </summary>
    public class CollectionConverter : ITypeConverter
    {
        private readonly MethodInfo enumerableCastMethod = typeof(Enumerable).GetMethod("Cast", BindingFlags.Public | BindingFlags.Static);
        private readonly MethodInfo enumerableToArrayMethod = typeof(Enumerable).GetMethod("ToArray", BindingFlags.Public | BindingFlags.Static);
        private readonly MethodInfo enumerableToListMethod = typeof(Enumerable).GetMethod("ToList", BindingFlags.Public | BindingFlags.Static);

        /// <summary>
        /// Whether the converter can convert to the destination type
        /// </summary>
        /// <param name="destinationType">Destination type</param>
        /// <param name="context">The current binding context</param>
        /// <returns>True if conversion supported, false otherwise</returns>
        public bool CanConvertTo(Type destinationType, BindingContext context)
        {
            return destinationType.IsCollection() || destinationType.IsEnumerable() || destinationType.IsArray();
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

            var items = input.Split(',');

            // Strategy, schmategy ;-)
            if (destinationType.IsCollection())
            {
                return this.ConvertCollection(items, destinationType, context);
            }

            if (destinationType.IsArray())
            {
                return this.ConvertArray(items, destinationType, context);
            }

            if (destinationType.IsEnumerable())
            {
                return this.ConvertEnumerable(items, destinationType, context);
            }

            return null;
        }

        private object ConvertCollection(string[] items, Type destinationType, BindingContext context)
        {
            var genericType = destinationType.GetGenericArguments().First();
            var returnCollection = Activator.CreateInstance(destinationType);

            var converter = context.TypeConverters.FirstOrDefault(c => c.CanConvertTo(genericType, context));
            if (converter == null)
            {
                return null;
            }

            var collectionAddMethod = destinationType.GetMethod("Add", BindingFlags.Public | BindingFlags.Instance);

            foreach (var item in items)
            {
                collectionAddMethod.Invoke(returnCollection, new[] { converter.Convert(item, genericType, context) });
            }

            return returnCollection;
        }

        private object ConvertArray(string[] items, Type destinationType, BindingContext context)
        {
            var elementType = destinationType.GetElementType();

            if (elementType == null)
            {
                return null;
            }

            var converter = context.TypeConverters.FirstOrDefault(c => c.CanConvertTo(elementType, context));

            if (converter == null)
            {
                return null;
            }

            var returnArray = items.Select(s => converter.Convert(s, elementType, context));

            var genericCastMethod = this.enumerableCastMethod.MakeGenericMethod(new[] { elementType });
            var generictoArrayMethod = this.enumerableToArrayMethod.MakeGenericMethod(new[] { elementType });

            var castArray = genericCastMethod.Invoke(null, new object[] { returnArray });

            return generictoArrayMethod.Invoke(null, new[] { castArray });
        }

        private object ConvertEnumerable(string[] items, Type destinationType, BindingContext context)
        {
            var genericType = destinationType.GetGenericArguments().First();

            var converter = context.TypeConverters.FirstOrDefault(c => c.CanConvertTo(genericType, context));

            if (converter == null)
            {
                return null;
            }

            var returnArray = items.Select(s => converter.Convert(s, genericType, context));

            // Use ToList rather than AsEnumerable to make sure the collection
            // is materialised and converters are called as appropriate.
            var genericCastMethod = this.enumerableCastMethod.MakeGenericMethod(new[] { genericType });
            var genericToListMethod = this.enumerableToListMethod.MakeGenericMethod(new[] { genericType });

            var castArray = genericCastMethod.Invoke(null, new object[] { returnArray });

            return genericToListMethod.Invoke(null, new[] { castArray });
        }
    }
}