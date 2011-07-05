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
                    RedirectUrl = "/login",
                    UsernameMapper = container.Resolve<IUsernameMapper>(),
                };

            FormsAuthentication.Enable(this, formsAuthConfiguration);
        }
    }
}