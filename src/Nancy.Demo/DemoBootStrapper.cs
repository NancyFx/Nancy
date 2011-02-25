namespace Nancy.Demo
{
    public class DemoBootstrapper : DefaultNancyBootstrapper
    {
        // Overriding this just to show how it works, not actually necessary as autoregister
        // takes care of it all.
        protected override void ConfigureApplicationContainer(TinyIoC.TinyIoCContainer existingContainer)
        {
            // We don't call base because we don't want autoregister
            // we just register our one known dependency as an application level singleton
            existingContainer.Register<IApplicationDependency, ApplicationDependencyClass>().AsSingleton();
        }

        public override void ConfigureRequestContainer(TinyIoC.TinyIoCContainer existingContainer)
<<<<<<< HEAD
        {
            base.ConfigureRequestContainer(existingContainer);

            existingContainer.Register<IRequestDependency, RequestDependencyClass>().AsSingleton();
=======
        {
            base.ConfigureRequestContainer(existingContainer);

            existingContainer.Register<IRequestDependency, RequestDependencyClass>().AsSingleton();
        }

        protected override void InitialiseInternal(TinyIoC.TinyIoCContainer container)
        {
            base.InitialiseInternal(container);

            this.AfterRequest += (ctx) =>
                {
                    var username = ctx.Request.Query.pirate;

                    if (username.HasValue)
                    {
                        ctx.Response = new HereBeAResponseYouScurvyDog(ctx.Response);
                    }
                };
>>>>>>> 4db860ef0fe65aa3c4b5ed52625c5ec03e369147
        }
    }
}