namespace Demo.Embedded
{
    using Nancy;
    using Nancy.Conventions;
    using Nancy.Embedded.Conventions;

    public class Bootstrapper : DefaultNancyBootstrapper
    {
        protected override void ConfigureConventions(NancyConventions nancyConventions)
        {
            // This will add a convention to serve embedded files from "/Content"
            nancyConventions.StaticContentsConventions.AddEmbeddedDirectory("/Content");
        }
    }
}