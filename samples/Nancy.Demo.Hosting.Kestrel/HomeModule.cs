namespace Nancy.Demo.Hosting.Kestrel
{
    using ModelBinding;

    public class HomeModule : NancyModule
    {
        public HomeModule(IAppConfiguration appConfig)
        {
            Get("/", args => "Hello from Nancy running on CoreCLR");

            Get("/conneg/{name}", args => new Person() { Name = args.name });

            Get("/smtp", args => 
            {
                return new
                {
                    appConfig.Smtp.Server,
                    appConfig.Smtp.User,
                    appConfig.Smtp.Pass,
                    appConfig.Smtp.Port
                };
            });

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
