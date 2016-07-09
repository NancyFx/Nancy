namespace Nancy.Demo.Hosting.Kestrel
{
    using ModelBinding;
    using System.Threading.Tasks;

    public class HomeModule : NancyModule
    {
        public HomeModule()
        {
            Get("/", args => "Hello from Nancy running on CoreCLR");

            Get("/conneg/{name}", args => new Person() { Name = args.name });

            Post("/", args =>
            {
                var person = this.BindAndValidate<Person>();

                if (!this.ModelValidationResult.IsValid)
                {
                    return 422;
                }

                return person;
            });
        }
    }
}
