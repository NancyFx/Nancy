namespace Nancy.ModelBinding.DefaultConverters
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Linq;
    using System.Reflection;

    /// <summary>
    /// Converter for handling enumerable types
    /// </summary>
    public class CollectionConverter : ITypeConverter
    {
        private readonly MethodInfo enumerableCastMethod = typeof(Enumerable).GetMethod("Cast", BindingFlags.Public | BindingFlags.Static);
        private readonly MethodInfo enumerableToArrayMethod = typeof(Enumerable).GetMethod("ToArray", BindingFlags.Public | BindingFlags.Static);
        private readonly MethodInfo enumerableAsEnumerableMethod = typeof(Enumerable).GetMethod("AsEnumerable", BindingFlags.Public | BindingFlags.Static);

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
            // TODO - Lots of reflection in here, should probably cache the methodinfos
            if (string.IsNullOrEmpty(input))
            {
                return null;
            }

            var items = input.Split(',');

            if (IsCollection(destinationType))
            {
                return ConvertCollection(items, destinationType, context);
            }

            if (IsArray(destinationType))
            {
                return ConvertArray(items, destinationType, context);
            }

            if (IsEnumerable(destinationType))
            {
                return ConvertEnumerable(items, destinationType, context);
            }

            return null;
        }

        private bool IsCollection(Type destinationType)
        {
            var collectionType = typeof(ICollection<>);

            return destinationType.IsGenericType && destinationType.GetInterfaces().
                Where(i => i.IsGenericType && i.GetGenericTypeDefinition() == collectionType).Any();
        }

        private bool IsArray(Type destinationType)
        {
            return destinationType.BaseType == typeof(Array);
        }

        private bool IsEnumerable(Type destinationType)
        {
            var enumerableType = typeof(IEnumerable<>);

            return destinationType.IsGenericType && destinationType.GetGenericTypeDefinition() == enumerableType;
        }

        private object ConvertCollection(string[] items, Type destinationType, BindingContext context)
        {
            var genericType = destinationType.GetGenericArguments().First();
            var returnCollection = Activator.CreateInstance(destinationType);

            var converter = context.TypeConverters.Where(c => c.CanConvertTo(genericType)).FirstOrDefault();
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

            var converter = context.TypeConverters.Where(c => c.CanConvertTo(elementType)).FirstOrDefault();

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

            var converter = context.TypeConverters.Where(c => c.CanConvertTo(genericType)).FirstOrDefault();

            if (converter == null)
            {
                return null;
            }

            var returnArray = items.Select(s => converter.Convert(s, genericType, context));

            var genericCastMethod = this.enumerableCastMethod.MakeGenericMethod(new[] { genericType });
            var genericAsEnumerableMethod = this.enumerableAsEnumerableMethod.MakeGenericMethod(new[] { genericType });

            var castArray = genericCastMethod.Invoke(null, new object[] { returnArray });

            return genericAsEnumerableMethod.Invoke(null, new[] { castArray });
        }
    }
}