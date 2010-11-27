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
                return new Response { Contents = "FakeNancyModuleWithBasePath" };
            };

            Get["/should/have/conflicting/route/defined"] = x => {
                return new Response { Contents = "FakeNancyModuleWithBasePath" };
            };

            Get["/child/{value}"] = x => {
                throw new NotImplementedException();
            };

            Get["/child/route/{value}"] = x => {
                return new Response { Contents = "test" };
            };

            Get["/"] = x => {
                throw new NotImplementedException();
            };

            Get["/foo/{value}/bar/{capture}"] = x => {
                throw new NotImplementedException();
            };

            Post["/"] = x => {
                return new Response { Contents = "Action result" };
            };

            Put["/"] = x => {
                throw new NotImplementedException();
            };
        }
    }
}