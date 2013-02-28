namespace Nancy.Bootstrapper
{
    using System;

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

        public Type ModuleType { get; private set; }
    }
}