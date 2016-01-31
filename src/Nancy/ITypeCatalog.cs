namespace Nancy
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Defines the functionality of a type catalog.
    /// </summary>
    public interface ITypeCatalog
    {
        /// <summary>
        /// Gets all types that are assignable to the provided <paramref name="type"/>.
        /// </summary>
        /// <param name="type">The <see cref="Type"/> that returned types should be assignable to.</param>
        /// <param name="strategy">A <see cref="TypeResolveStrategy"/> that should be used then retrieving types.</param>
        /// <returns>An <see cref="IReadOnlyCollection{T}"/> of <see cref="Type"/> instances.</returns>
        IReadOnlyCollection<Type> GetTypesAssignableTo(Type type, TypeResolveStrategy strategy);

        /// <summary>
        /// Gets all types that are assignable to the provided <typeparamref name="TType"/>.
        /// </summary>
        /// <typeparam name="TType">The <see cref="Type"/> that returned types should be assignable to.</typeparam>
        /// <param name="strategy">A <see cref="TypeResolveStrategy"/> that should be used then retrieving types.</param>
        /// <returns>An <see cref="IReadOnlyCollection{T}"/> of <see cref="Type"/> instances.</returns>
        IReadOnlyCollection<Type> GetTypesAssignableTo<TType>(TypeResolveStrategy strategy);
    }
}
