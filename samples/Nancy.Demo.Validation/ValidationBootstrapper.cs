namespace Nancy.Demo.Validation
{
    using Nancy.TinyIoc;

    public class ValidationBootstrapper : DefaultNancyBootstrapper
    {
        protected override void ConfigureApplicationContainer(TinyIoCContainer container)
        {
            // Disable auto-registration so that we can make sure that the
            // application registrations are preformed correctly by each of
            // the validation projects. This is for testing purposes only
            // and is not required to perform in your own project.

            //base.ConfigureApplicationContainer(container);
        }
    }
}