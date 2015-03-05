namespace Nancy.Bootstrapper
{
    /// <summary>
    /// Determines which set of types that the <see cref="AppDomainAssemblyTypeScanner"/> should scan in.
    /// </summary>
    public enum ScanMode
    {
        /// <summary>
        /// All available types.
        /// </summary>
        All,

        /// <summary>
        /// Only in types that are defined in the Nancy assembly.
        /// </summary>
        OnlyNancy,

        /// <summary>
        /// Only types that are defined outside the Nancy assembly.
        /// </summary>
        ExcludeNancy,

        /// <summary>
        /// Only Namespaces that starts with 'Nancy'
        /// </summary>
        OnlyNancyNamespace,

        /// <summary>
        /// Only Namespaces that does not start with Nancy
        /// </summary>
        ExcludeNancyNamespace
    }
}