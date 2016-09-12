namespace Nancy.Session
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Provides <see cref="SessionProvider"/> with dependencies
    /// </summary>
    public interface ISessionDependency
    {
        /// <summary>
        /// Returns type dependencies to SessionProvider for instantiation
        /// </summary>
        Func<IReadOnlyCollection<Type>> GetTypeDependencies { get; }

        /// <summary>
        ///  Returns type instance dependencies to SessionProvider
        /// </summary>
        Func<IReadOnlyCollection<object>> GetInstancedDependencies { get; }
    }
}