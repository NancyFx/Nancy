namespace Nancy.Demo.Authentication.Forms
{
    using Nancy;
    using Nancy.Authentication.Forms;

    public class FormsAuthBootstrapper : DefaultNancyBootstrapper
    {
        protected override void InitialiseInternal(TinyIoC.TinyIoCContainer container)
        {
            base.InitialiseInternal(container);

            var formsAuthConfiguration = 
                new FormsAuthenticationConfiguration()
                {
                    RedirectUrl = "~/login",
                    UserMapper = container.Resolve<IUserMapper>(),
                };

            FormsAuthentication.Enable(this, formsAuthConfiguration);
        }
    }
}