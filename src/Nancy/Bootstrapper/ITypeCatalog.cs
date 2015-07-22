namespace Nancy.Bootstrapper
{
    using System;
    using System.Collections.Generic;
    using System.Reflection;

    /// <summary>
    /// Defines the functionality for getting all types implementing a particular interface/base class
    /// </summary>
    public interface ITypeCatalog
    {
        /// <summary>
        /// Gets all types implementing a particular interface/base class
        /// </summary>
        /// <param name="type">Type to search for</param>
        /// <returns>An <see cref="IEnumerable{T}"/> of types.</returns>
        /// <remarks>Will scan with <see cref="ScanMode.All"/>.</remarks>
        IEnumerable<Type> TypesOf(Type type);

        /// <summary>
        /// Gets all types implementing a particular interface/base class
        /// </summary>
        /// <param name="type">Type to search for</param>
        /// <param name="mode">A <see cref="ScanMode"/> value to determine which type set to scan in.</param>
        /// <returns>An <see cref="IEnumerable{T}"/> of types.</returns>
        IEnumerable<Type> TypesOf(Type type, ScanMode mode);

        /// <summary>
        /// Gets all types implementing a particular interface/base class
        /// </summary>
        /// <typeparam name="TType">Type to search for</typeparam>
        /// <returns>An <see cref="IEnumerable{T}"/> of types.</returns>
        /// <remarks>Will scan with <see cref="ScanMode.All"/>.</remarks>
        IEnumerable<Type> TypesOf<TType>();

        /// <summary>
        /// Gets all types implementing a particular interface/base class
        /// </summary>
        /// <typeparam name="TType">Type to search for</typeparam>
        /// <param name="mode">A <see cref="ScanMode"/> value to determine which type set to scan in.</param>
        /// <returns>An <see cref="IEnumerable{T}"/> of types.</returns>
        IEnumerable<Type> TypesOf<TType>(ScanMode mode);
    }

    public interface IAssemblyCatalog
    {
        IEnumerable<Assembly> Assemblies { get; } 
    }

    public class DefaultAssemblyCatalog : IAssemblyCatalog
    {
        public IEnumerable<Assembly> Assemblies
        {
            get { return AppDomainAssemblyTypeScanner.Assemblies; }
        }
    }
}