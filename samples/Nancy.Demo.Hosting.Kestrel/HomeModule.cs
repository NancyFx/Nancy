namespace Nancy.Demo.Hosting.Kestrel
{
    using System.Threading.Tasks;

    public class HomeModule : NancyModule
    {
        public HomeModule()
        {
            Get("/", (args, ct) => Task.FromResult("Hello from Nancy running on CoreCLR"));

            Get("/conneg/{name}", (args, token) => Task.FromResult(new Person() { Name = args.name }));
        }
    }
}
