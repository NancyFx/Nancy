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
        /// <param name="strategy">A <see cref="TypeResolveStrategy"/> that should be used when retrieving types.</param>
        /// <returns>An <see cref="IReadOnlyCollection{T}"/> of <see cref="Type"/> instances.</returns>
        IReadOnlyCollection<Type> GetTypesAssignableTo(Type type, TypeResolveStrategy strategy);
    }
}
