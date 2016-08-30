namespace Nancy.Bootstrapper
{
    using System;

    /// <summary>
    /// Holds module type for registration into a container.
    /// </summary>
    public sealed class ModuleRegistration
    {

        /// <summary>
        /// Initializes a new instance of the <see cref="ModuleRegistration"/> class, with
        /// the provided <paramref name= "moduleType" />
        /// </summary>
        /// <param name="moduleType">Type of the module.</param>
        public ModuleRegistration(Type moduleType)
        {
            ModuleType = moduleType;
        }

        /// <summary>
        /// Gets the type of the module.
        /// </summary>
        /// <value> The type of the module.</value>
        public Type ModuleType { get; private set; }
    }
}