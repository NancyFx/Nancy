namespace Nancy.Demo
{
    using ViewEngines;
    using ViewEngines.NDjango;
    using ViewEngines.Razor;
    using ViewEngines.Spark;

    public class DemoBootstrapper : DefaultNancyBootstrapper
    {
        // Overriding this just to show how it works, not actually necessary as autoregister
        // takes care of it all.
        protected override void ConfigureApplicationContainer(TinyIoC.TinyIoCContainer container)
        {
            // We don't call base because we don't want autoregister
            // we just register our one known dependency as an application level singleton
            container.Register<IApplicationDependency, ApplicationDependencyClass>().AsSingleton();

            container.RegisterMultiple<IViewEngineRegistry>(new[]
                                                                {
                                                                    typeof (SparkViewRegistry),
                                                                    typeof (RazorViewRegistry),
                                                                    typeof (NDjangoViewRegistry)
                                                                }).AsSingleton();
        }
        
        public override void ConfigureRequestContainer(TinyIoC.TinyIoCContainer container)
        {
            base.ConfigureRequestContainer(container);

            container.Register<RequestDependency, RequestDependencyClass>().AsSingleton();
        }
    }
}