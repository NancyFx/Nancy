namespace Nancy.Demo
{
    using Session;

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
        {
            base.ConfigureRequestContainer(existingContainer);

            existingContainer.Register<IRequestDependency, RequestDependencyClass>().AsSingleton();
        }

        protected override void InitialiseInternal(TinyIoC.TinyIoCContainer container)
        {
            base.InitialiseInternal(container);

            CookieSessionStore.Passphrase = "This is my really s349 secure P213#(al passphrase";
            CookieSessionStore.Salt = "And *232 also 438 my salt!!";

            this.AfterRequest += (ctx) =>
                {
                    var username = ctx.Request.Query.pirate;

                    if (username.HasValue)
                    {
                        ctx.Response = new HereBeAResponseYouScurvyDog(ctx.Response);
                    }
                };

            this.AfterRequest += (ctx) => new CookieSessionStore().Save(ctx.Request.Session, ctx.Response);
        }
    }
}