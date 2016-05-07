namespace Nancy.Tests.Fakes
{
    public class FakeNancyModuleWithoutBasePath : NancyModule
    {
        public FakeNancyModuleWithoutBasePath()
        {
            Delete("/", args => {
                return "Default delete root";
            });

            Get("/", args => {
                return "Default get root";
            });

            Get("/fake/should/have/conflicting/route/defined", args => {
                return "FakeNancyModuleWithoutBasePath";
            });

            Get("/greet/{name}", args =>
            {
                return string.Concat("Hello ", args.name);
            });

            Get("/filtered",
                condition: req => false,
                action: args =>
                {
                    return "I should never be run because I am filtered";
                });

            Get("/notfiltered",
                condition: req => true,
                action: args =>
                {
                    return "I should always be fine because my filter returns true";
                });

            Post("/", args => {
                return "Default post root";
            });

            Put("/", args => {
                return "Default put root";
            });

            Get("/filt",
                condition: req => false,
                action: args =>
                {
                    return "false";
                });

            Get("/filt",
                condition: req => true,
                action: args =>
                {
                    return "true";
                });

            Get("/filt",
                condition: req => false,
                action: args =>
                {
                    return "false";
                });
        }
    }
}
