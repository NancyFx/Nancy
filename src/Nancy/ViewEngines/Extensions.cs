namespace Nancy.ViewEngines
{
    using System;
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
            if (source == null)
            {
                return false;
            }

            var type = source.GetType();

            return type.IsGenericType 
                   && (type.Attributes & TypeAttributes.NotPublic) == TypeAttributes.NotPublic
                   && (type.Name.StartsWith("<>", StringComparison.OrdinalIgnoreCase) || type.Name.StartsWith("VB$", StringComparison.OrdinalIgnoreCase))
                   && type.Name.Contains("AnonymousType")
                   && Attribute.IsDefined(type, typeof (CompilerGeneratedAttribute), false);
        }
    }
}