namespace Nancy.Tests.Fakes
{
    using System;
    using Nancy;

    public class FakeNancyModule : NancyModule
    {
        public FakeNancyModule() : base("/fake")
        {
            Delete["/"] = x => {
                throw new NotImplementedException();
            };

            Get["/route/with/some/parts"] = x => {
                return new Response();
            };

            Get["/child/{value}"] = x => {
                throw new NotImplementedException();
            };

            Get["/child/route/{value}"] = x => {
                throw new NotImplementedException();
            };

            Get["/"] = x => {
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

    public class FakeNancyModule2 : NancyModule
    {
        
    }
}