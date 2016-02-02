namespace Nancy
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Contains extension methods for <see cref="ITypeCatalog"/> implementations.
    /// </summary>
    public static class TypeCatalogExtensions
    {
        /// <summary>
        /// Gets all <see cref="Type"/> instances that are assignable to <paramref name="type"/>, using <see cref="TypeResolveStrategies.All"/>.
        /// </summary>
        /// <param name="typeCatalog">The <see cref="ITypeCatalog"/> instance where the types should be retrieved from.</param>
        /// <param name="type">The <see cref="Type"/> that all returned types should be assingable to.</param>
        /// <returns>An <see cref="IReadOnlyCollection{T}"/> of <see cref="Type"/> instances.</returns>
        public static IReadOnlyCollection<Type> GetTypesAssignableTo(this ITypeCatalog typeCatalog, Type type)
        {
            return typeCatalog.GetTypesAssignableTo(type, TypeResolveStrategies.All);
        }

        /// <summary>
        /// Gets all <see cref="Type"/> instances that are assignable to <typeparamref name="TType"/>, using <see cref="TypeResolveStrategies.All"/>.
        /// </summary>
        /// <param name="typeCatalog">The <see cref="ITypeCatalog"/> instance where the types should be retrieved from.</param>
        /// <typeparam name="TType">The <see cref="Type"/> that all returned types should be assingable to.</typeparam>
        /// <returns>An <see cref="IReadOnlyCollection{T}"/> of <see cref="Type"/> instances.</returns>
        public static IReadOnlyCollection<Type> GetTypesAssignableTo<TType>(this ITypeCatalog typeCatalog)
        {
            return typeCatalog.GetTypesAssignableTo(typeof(TType), TypeResolveStrategies.All);
        }

        /// <summary>
        /// Gets all types that are assignable to the provided <typeparamref name="TType"/>.
        /// </summary>
        /// <param name="typeCatalog">The <see cref="ITypeCatalog"/> instance where the types should be retrieved from.</param>
        /// <param name="strategy">A <see cref="TypeResolveStrategy"/> that should be used then retrieving types.</param>
        /// <typeparam name="TType">The <see cref="Type"/> that returned types should be assignable to.</typeparam>
        /// <returns>An <see cref="IReadOnlyCollection{T}"/> of <see cref="Type"/> instances.</returns>
        public static IReadOnlyCollection<Type> GetTypesAssignableTo<TType>(this ITypeCatalog typeCatalog, TypeResolveStrategy strategy)
        {
            return typeCatalog.GetTypesAssignableTo(typeof(TType), strategy);
        }
    }
}
