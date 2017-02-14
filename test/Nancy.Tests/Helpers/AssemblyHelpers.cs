namespace Nancy.Tests.Helpers
{
    using System.IO;
    using System.Reflection;

#if CORE
    using System.Runtime.Loader;
#endif

    /// <summary>
    /// Convenience class with helper methods for <see cref="Assembly"/>.
    /// </summary>
    public static class AssemblyHelpers
    {
        /// <summary>
        /// Loads an assembly from the emitted image contained in <paramref name="stream"/>.
        /// </summary>
        /// <param name="stream">The steam that contains the emitted assembly.</param>
        /// <returns>The loaded assembly.</returns>
        public static Assembly Load(MemoryStream stream)
        {
#if CORE
            stream.Position = 0;
            return AssemblyLoadContext.Default.LoadFromStream(stream);
#else
            return Assembly.Load(stream.ToArray());
#endif

        }
    }
}
