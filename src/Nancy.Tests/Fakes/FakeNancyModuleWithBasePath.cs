namespace Nancy.Tests.Fakes
{
    using System;
    using Nancy;

    public class FakeNancyModuleWithBasePath : NancyModule
    {
        public FakeNancyModuleWithBasePath() : base("/fake")
        {
            Delete["/"] = x => {
                throw new NotImplementedException();
            };

            Get["/route/with/some/parts"] = x => {
                return "FakeNancyModuleWithBasePath";
            };

            Get["/should/have/conflicting/route/defined"] = x => {
                return "FakeNancyModuleWithBasePath";
            };

            Get["/child/{value}"] = x => {
                throw new NotImplementedException();
            };

            Get["/child/route/{value}"] = x => {
                return "test";
            };

            Get["/"] = x => {
                throw new NotImplementedException();
            };

            Get["/foo/{value}/bar/{capture}"] = x => {
                throw new NotImplementedException();
            };

            Post["/"] = x => {
                return "Action result";
            };

            Put["/"] = x => {
                throw new NotImplementedException();
            };
        }
    }
}