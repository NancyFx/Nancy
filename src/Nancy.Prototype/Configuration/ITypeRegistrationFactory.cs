namespace Nancy.Prototype.Configuration
{
    using System;
    using Nancy.Prototype.Registration;

    public interface ITypeRegistrationFactory<in TService> : IRegistrationFactory<TypeRegistration>
    {
        void Use<TImplementation>() where TImplementation : TService;

        void Use(Type implementationType);
    }
}
