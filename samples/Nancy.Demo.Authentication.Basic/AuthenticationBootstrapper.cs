namespace Nancy.Demo.Authentication.Basic
{
    using Nancy.Authentication.Basic;
    using Nancy.Bootstrapper;
    using Nancy.TinyIoc;

    public class AuthenticationBootstrapper : DefaultNancyBootstrapper
    {
        protected override void ApplicationStartup(TinyIoCContainer container, IPipelines pipelines)
        {
            base.ApplicationStartup(container, pipelines);

            pipelines.EnableBasicAuthentication(new BasicAuthenticationConfiguration(
                container.Resolve<IUserValidator>(),
                "MyRealm"));
        }
    }
}