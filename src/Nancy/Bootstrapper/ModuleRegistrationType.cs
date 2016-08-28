namespace Nancy.Bootstrapper
{
    using System;

    /// <summary>
    /// Holds module type for registration into a container.
    /// </summary>
    public sealed class ModuleRegistration
    {
        /// <summary>
        /// Represents a module type for registration into a container
        /// </summary>
        /// <param name="moduleType">Type of the module</param>
        public ModuleRegistration(Type moduleType)
        {
            ModuleType = moduleType;
        }

        /// <summary>
        /// Gets the type of the module.
        /// </summary>
        /// <value>
        /// The type of the module.
        /// </value>
        public Type ModuleType { get; private set; }
    }
}