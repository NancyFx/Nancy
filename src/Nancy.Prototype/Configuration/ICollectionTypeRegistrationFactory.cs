namespace Nancy.Prototype.Configuration
{
    using System;
    using Nancy.Prototype.Registration;

    public interface ICollectionTypeRegistrationFactory<in TService> : IRegistrationFactory<CollectionTypeRegistration>
    {
        void Use<TImplementation>() where TImplementation : TService;

        void Use(Type implementationType);
    }
}
