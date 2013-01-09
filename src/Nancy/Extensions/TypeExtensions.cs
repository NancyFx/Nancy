namespace Nancy.Extensions
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public static class TypeExtensions
    {
        public static string GetAssemblyPath(this Type source)
        {
            var assemblyUri =
                new Uri(source.Assembly.EscapedCodeBase);

            return assemblyUri.LocalPath;
        }

        public static bool IsArray(this Type source)
        {
            return source.BaseType == typeof(Array);
        }

        public static bool IsCollection(this Type source)
        {
            var collectionType = typeof(ICollection<>);

            return source.IsGenericType && source
                .GetInterfaces()
                .Any(i => i.IsGenericType && i.GetGenericTypeDefinition() == collectionType);
        }

        public static bool IsEnumerable(this Type source)
        {
            var enumerableType = typeof(IEnumerable<>);

            return source.IsGenericType && source.GetGenericTypeDefinition() == enumerableType;
        }

        /// <summary>
        /// Determines if a type is numeric.  Nullable numeric types are considered numeric.
        /// </summary>
        /// <remarks>
        /// Boolean is not considered numeric.
        /// </remarks>
        public static bool IsNumeric(this Type source)
        {
            if (source == null)
            {
                return false;
            }

            switch (Type.GetTypeCode(source))
            {
                case TypeCode.Byte:
                case TypeCode.Decimal:
                case TypeCode.Double:
                case TypeCode.Int16:
                case TypeCode.Int32:
                case TypeCode.Int64:
                case TypeCode.SByte:
                case TypeCode.Single:
                case TypeCode.UInt16:
                case TypeCode.UInt32:
                case TypeCode.UInt64:
                    return true;
                case TypeCode.Object:
                    if (source.IsGenericType && source.GetGenericTypeDefinition() == typeof(Nullable<>))
                    {
                        return IsNumeric(Nullable.GetUnderlyingType(source));
                    }
                    return false;
            }
            return false;
        }
    }
}