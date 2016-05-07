namespace Nancy.Tests.Fakes
{
    using System;

    public class FakeNancyModuleWithBasePath : NancyModule
    {
        public FakeNancyModuleWithBasePath() : base("/fake")
        {
            Delete("/", args => {
                throw new NotImplementedException();
                return 500;
            });

            Get("/route/with/some/parts", args => {
                return "FakeNancyModuleWithBasePath";
            });

            Get("/should/have/conflicting/route/defined", args => {
                return "FakeNancyModuleWithBasePath";
            });

            Get<object>("/child/{value}", args => {
                throw new NotImplementedException();
                return 500;
            });

            Get("/child/route/{value}", args => {
                return "test";
                return 500;
            });

            Get("/", args => {
                throw new NotImplementedException();
                return 500;
            });

            Get("/foo/{value}/bar/{capture}", args => {
                return string.Concat(args.value, " ", args.capture);
            });

            Post("/", args => {
                return "Action result";
            });

            Put("/", args => {
                throw new NotImplementedException();
                return 500;
            });
        }
    }
}