namespace Nancy.Prototype.Registration
{
    using System;

    public class InstanceRegistration : ContainerRegistration
    {
        public InstanceRegistration(Type serviceType, object instance)
            : base(serviceType, Lifetime.Singleton)
        {
            Check.NotNull(instance, nameof(instance));

            this.Instance = instance;
        }

        public object Instance { get; }
    }
}
