namespace Nancy.Prototype.Registration
{
    using System;

    public abstract class ContainerRegistration
    {
        protected ContainerRegistration(Type serviceType, Lifetime lifetime)
        {
            Check.NotNull(serviceType, nameof(serviceType));

            this.ServiceType = serviceType;
            this.Lifetime = lifetime;
        }

        public Type ServiceType { get; }

        public Lifetime Lifetime { get; }
    }
}
