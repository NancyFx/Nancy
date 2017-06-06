namespace Nancy.Prototype.Registration
{
    using System;

    public class TypeRegistration : ContainerRegistration
    {
        public TypeRegistration(Type serviceType, Type implementationType, Lifetime lifetime)
            : base(serviceType, lifetime)
        {
            Check.NotNull(implementationType, nameof(implementationType));

            this.ImplementationType = implementationType;
        }

        public Type ImplementationType { get; }
    }
}
