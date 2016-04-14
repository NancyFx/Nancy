namespace Nancy.Demo.Hosting.Kestrel
{
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.Http;
    using Nancy.Owin;
    
    public class Program
    {
        public static void Main(string[] args)
        {
            var host = new WebHostBuilder()
                .UseKestrel()
                .Configure(app => app.UseOwin(x => x.UseNancy()))
                .Build();

            host.Run();
        }
    }
}
