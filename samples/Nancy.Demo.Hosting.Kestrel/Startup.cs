namespace Nancy.Demo.Hosting.Kestrel
{
    using Microsoft.AspNet.Builder;
    using Microsoft.AspNet.Hosting;
    using Nancy.Owin;

    public class Startup
    {
        public void Configure(IApplicationBuilder app)
        {
            app.UseOwin(x => x.UseNancy());
        }

        public static void Main(string[] args) => WebApplication.Run<Startup>(args);
    }
}
