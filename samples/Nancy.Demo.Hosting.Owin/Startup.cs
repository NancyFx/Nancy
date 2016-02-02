namespace Nancy.Demo.Hosting.Owin
{
    using global::Owin;

    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            app.UseNancy();
        }
    }
}