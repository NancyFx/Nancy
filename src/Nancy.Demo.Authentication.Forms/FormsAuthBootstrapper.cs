namespace Nancy.Demo.Authentication.Forms
{
    using Nancy;
    using Nancy.Authentication.Forms;

    public class FormsAuthBootstrapper : DefaultNancyBootstrapper
    {
        protected override void ConfigureApplicationContainer(TinyIoC.TinyIoCContainer container)
        {
        }

        protected override void InitialiseInternal(TinyIoC.TinyIoCContainer container, Bootstrapper.IPipelines pipelines)
        {
        }

        protected override void ConfigureRequestContainer(TinyIoC.TinyIoCContainer container)
        {
            base.ConfigureRequestContainer(container);

            container.Register<IUserMapper, UserDatabase>();
        }

        protected override void InitialiseRequestInternal(TinyIoC.TinyIoCContainer requestContainer, Bootstrapper.IPipelines pipelines)
        {
            var formsAuthConfiguration =
                new FormsAuthenticationConfiguration()
                {
                    RedirectUrl = "~/login",
                    UserMapper = requestContainer.Resolve<IUserMapper>(),
                };

            FormsAuthentication.Enable(pipelines, formsAuthConfiguration);
        }
    }
}