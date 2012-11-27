namespace Nancy.Bootstrapper
{
    /// <summary>
    /// Determins which set of types that the <see cref="AppDomainAssemblyTypeScanner"/> should scan in.
    /// </summary>
    public enum ScanMode
    {
        /// <summary>
        /// All available types.
        /// </summary>
        All,

        /// <summary>
        /// Only in types that are defined in Nancy assemblies.
        /// </summary>
        OnlyNancy,

        /// <summary>
        /// Only types that are defined outside Nancy assemblies.
        /// </summary>
        ExcludeNancy
    }
}