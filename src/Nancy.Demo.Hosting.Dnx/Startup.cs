
namespace Nancy.Demo.Hosting.Dnx
{
    using Microsoft.AspNet.Builder;
    using Microsoft.Framework.DependencyInjection;

    using Nancy.Owin;

    public class Startup
    {
        public void Configure(IApplicationBuilder app)
        {
            app.UseOwin().UseNancy();
        }
    }
}
