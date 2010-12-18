namespace Nancy
{
    using System;

    public class DefaultModuleActivator : IModuleActivator
    {
        /// <summary>
        /// Creates an instance of the specified <see cref="Type"/>.
        /// </summary>
        /// <param name="moduleType">The <see cref="Type"/> of the module to instantiate.</param>
        /// <returns>A <see cref="NancyModule"/> instance.</returns>
        public virtual NancyModule CreateInstance(Type moduleType)
        {
            if(!CanCreateInstance(moduleType))
            {
                throw new InvalidOperationException("Cannot create an instance of type {0} as it does not inherit from NancyModule or it does not have a public parameterless constructor.");
            }

            return (NancyModule) Activator.CreateInstance(moduleType);
        }

        /// <summary>
        /// Checks whether the activator can create an instance of the specified type.
        /// </summary>
        /// <param name="moduleType">The <see cref="Type"/> to check.</param>
        /// <returns><see langword="true"/> if this activator can create an instance of the specified module type, otherwise <see langword="false"/>.</returns>
        public virtual bool CanCreateInstance(Type moduleType)
        {
            return moduleType.GetConstructor(Type.EmptyTypes) != null;
        }
    }
}