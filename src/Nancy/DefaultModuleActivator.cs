namespace Nancy
{
    using System;

    public class DefaultModuleActivator : IModuleActivator
    {
        public virtual NancyModule CreateInstance(Type moduleType)
        {
            if(! CanCreateInstance(moduleType))
            {
                throw new InvalidOperationException("Cannot create an instance of type {0} as it does not inherit from NancyModule or it does not have a public parameterless constructor.");
            }

            return (NancyModule) Activator.CreateInstance(moduleType);
        }

        public virtual bool CanCreateInstance(Type moduleType)
        {
            bool hasDefaultConstructor = moduleType.GetConstructor(Type.EmptyTypes) != null;
            return hasDefaultConstructor;
        }
    }
}