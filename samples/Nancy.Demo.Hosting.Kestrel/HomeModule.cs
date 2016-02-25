namespace Nancy.Demo.Hosting.Kestrel
{
    using System.Threading.Tasks;

    public class HomeModule : NancyModule
    {
        public HomeModule()
        {
            Get["/"] = (o, token) => Task.FromResult<dynamic>("Hello from Nancy running on CoreCLR");

            Get["/conneg/{name}"] = (parameters, token) => Task.FromResult<dynamic>(new Person() { Name = parameters.name });
        }
    }
}
