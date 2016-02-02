namespace Nancy
{
    using System.Reflection;

    /// <summary>
    /// Predicate used to decide if a <see cref="Assembly"/> should be included when resolving assemblies.
    /// </summary>
    /// <param name="assembly">The <see cref="Assembly"/> that is being inspected.</param>
    /// <value><see langword="true"/> if the assembly should be included in the result, otherwise <see langword="false"/>.</value>
    public delegate bool AssemblyResolveStrategy(Assembly assembly);
}
