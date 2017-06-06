namespace Nancy.Prototype.Configuration
{
    using Nancy.Prototype.Registration;
    using Nancy.Prototype.Scanning;

    public interface IRegistrationFactory<out TRegistration> where TRegistration : ContainerRegistration
    {
        TRegistration GetRegistration(ITypeCatalog typeCatalog);
    }
}
