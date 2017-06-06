namespace Nancy.Prototype.Registration
{
    using System;
    using System.Collections.Generic;

    public class CollectionTypeRegistration : ContainerRegistration
    {
        public CollectionTypeRegistration(Type serviceType, IReadOnlyCollection<Type> implementationTypes, Lifetime lifetime)
            : base(serviceType, lifetime)
        {
            Check.NotNull(implementationTypes, nameof(implementationTypes));

            this.ImplementationTypes = implementationTypes;
        }

        public IReadOnlyCollection<Type> ImplementationTypes { get; }
    }
}
