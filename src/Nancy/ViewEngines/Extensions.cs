namespace Nancy.ViewEngines
{
    using System;
    using System.Linq;
    using System.Reflection;
    using System.Runtime.CompilerServices;

    /// <summary>
    /// Contains miscellaneous extension methods.
    /// </summary>
    public static class Extensions
    {
        /// <summary>
        /// Checks if the evaluated instance is an anonymous
        /// </summary>
        /// <param name="source">The object instance to check.</param>
        /// <returns><see langword="true"/> if the object is an anonymous type; otherwise <see langword="false"/>.</returns>
        public static bool IsAnonymousType(this object source)
        {
            return source != null && source.GetType().IsAnonymousType();
        }

        /// <summary>
        /// Determines whether [is anonymous type].
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns><see langword="true"/> if type is anonymous</returns>
        public static bool IsAnonymousType(this Type type)
        {
            if (type == null)
            {
                return false;
            }

            return type.GetTypeInfo().IsGenericType
                   && (type.GetTypeInfo().Attributes & TypeAttributes.NotPublic) == TypeAttributes.NotPublic
                   && (type.Name.StartsWith("<>", StringComparison.OrdinalIgnoreCase) || type.Name.StartsWith("VB$", StringComparison.OrdinalIgnoreCase))
                   && (type.Name.Contains("AnonymousType") || type.Name.Contains("AnonType"))
                   && type.GetTypeInfo().GetCustomAttributes(typeof(CompilerGeneratedAttribute)).Any();
        }
    }
}