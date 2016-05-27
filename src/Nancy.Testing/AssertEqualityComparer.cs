namespace Nancy.Testing
{
    using System;
    using System.Collections.Generic;
    using System.Reflection;

    public class AssertEqualityComparer<T> : IEqualityComparer<T>
    {
        private static bool IsTypeNullable(Type type)
        {
            return type.GetTypeInfo().IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>);
        }

        public bool Equals(T expected, T actual)
        {
            var type =
                typeof(T);

            if (!type.GetTypeInfo().IsValueType || IsTypeNullable(type))
            {
                var actualIsNull =
                    (Object.Equals(actual, default(T)));

                var expectedIsNull =
                    (Object.Equals(expected, default(T)));

                if (actualIsNull || expectedIsNull)
                {
                    return false;
                }
            }

            var equality = actual as IEquatable<T>;
            if (equality != null)
            {
                return equality.Equals(expected);
            }

            var genericComparable = actual as IComparable<T>;
            if (genericComparable != null)
            {
                return genericComparable.CompareTo(expected) == 0;
            }

            var comparable = actual as IComparable;
            if (comparable != null)
            {
                return comparable.CompareTo(expected) == 0;
            }

            return false;
        }

        public int GetHashCode(T actual)
        {
            throw new NotSupportedException();
        }
    }
}