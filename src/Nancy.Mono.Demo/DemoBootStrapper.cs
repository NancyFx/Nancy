namespace Nancy.Demo
{
    public class DemoBootStrapper : DefaultNancyBootstrapper
    {
        // Overriding this just to show how it works, not actually necessary as autoregister
        // takes care of it all.
        protected override void ConfigureApplicationContainer(TinyIoC.TinyIoCContainer container)
        {
            // We don't call base because we don't want autoregister
            // we just register our one known dependency as an application level singleton
            container.Register<IApplicationDependency, ApplicationDependencyClass>().AsSingleton();
        }
        
        public override void ConfigureRequestContainer(TinyIoC.TinyIoCContainer container)
        {
            base.ConfigureRequestContainer(container);

            container.Register<IRequestDependency, RequestDependencyClass>().AsSingleton();
        }
		
//		protected override void InitialiseInternal(TinyIoC.TinyIoCContainer container)
//        {
//            base.InitialiseInternal(container);
//
//            this.AfterRequest += (ctx) =>
//                {
//                    var username = ctx.Request.Query.pirate;
//
//                    if (username.HasValue)
//                    {
//                        ctx.Response = new HereBeAResponseYouScurvyDog(ctx.Response);
//                    }
//                };
//        }
    }
}