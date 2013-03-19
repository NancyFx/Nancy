namespace Nancy.Demo.MarkdownViewEngine
{
    using Nancy.Bootstrapper;
    using Session;
    using TinyIoc;
    using ViewEngines;

    public class Bootstrapper : DefaultNancyBootstrapper
    {
        protected override void ConfigureApplicationContainer(TinyIoCContainer container)
        {
            base.ConfigureApplicationContainer(container);
            container.Register<IFileSystemReader, DefaultFileSystemReader>();
        }

        protected override void ApplicationStartup(TinyIoCContainer container, IPipelines pipelines)
        {
            CookieBasedSessions.Enable(pipelines);
        }
    }
}