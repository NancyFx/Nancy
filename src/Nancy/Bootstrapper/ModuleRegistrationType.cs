namespace Nancy.Bootstrapper
{
    using System;

    public sealed class ModuleRegistration
    {
        /// <summary>
        /// Represents a module type for registration into a container
        /// </summary>
        /// <param name="moduleType">Type of the module</param>
        /// <param name="moduleKey">Key that can be used to retrieve the specific module via <see cref="INancyModuleCatalog.GetModuleByKey"/></param>
        public ModuleRegistration(Type moduleType, string moduleKey)
        {
            ModuleType = moduleType;
            ModuleKey = moduleKey;
        }

        public string ModuleKey { get; private set; }

        public Type ModuleType { get; private set; }
    }
}