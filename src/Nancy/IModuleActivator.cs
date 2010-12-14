namespace Nancy
{
    using System;

    /// <summary>
    /// Used to create an instance of a module.
    /// </summary>
    public interface IModuleActivator
    {
        /// <summary>
        /// Creates an instance of the module with the specified type.
        /// </summary>
        /// <param name="moduleType">The type of the module to activate</param>
        /// <returns>The instantiated module</returns>
        NancyModule CreateInstance(Type moduleType);

        /// <summary>
        /// Checks whether the activator can create an instance of the specified type. 
        /// </summary>
        /// <param name="moduleType">The type of the module to check</param>
        /// <returns>True if this activator can create an instance of the specified module type, otherwise false.</returns>
        bool CanCreateInstance(Type moduleType);
    }
}