namespace Nancy.Demo.Hosting.Kestrel
{
    using ModelBinding;
    using System.Threading.Tasks;

    public class HomeModule : NancyModule
    {
        public HomeModule()
        {
            Get("/", (args, ct) => Task.FromResult("Hello from Nancy running on CoreCLR"));

            Get("/conneg/{name}", (args, token) => Task.FromResult(new Person() { Name = args.name }));

            Post("/", (parameters, token) =>
            {
                var person = this.BindAndValidate<Person>();

                if (!this.ModelValidationResult.IsValid)
                {
                    return Task.FromResult<dynamic>(422);
                }

                return Task.FromResult<dynamic>(person);
            });
        }
    }
}
