namespace Nancy
{
    using System;

    /// <summary>
    /// Predicate used to decide if a <see cref="Type"/> should be included when resolving types.
    /// </summary>
    /// <param name="type">The <see cref="Type"/> that is being inspected.</param>
    /// <value><see langword="true"/> if the type should be included in the result, otherwise <see langword="false"/>.</value>
    public delegate bool TypeResolveStrategy(Type type);
}
