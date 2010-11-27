namespace Nancy.Tests.Fakes
{
    using System;

    public class FakeNancyModuleWithoutBasePath : NancyModule
    {
        public FakeNancyModuleWithoutBasePath()
        {
            Delete["/"] = x => {
                return "Default delete root";
            };

            Get["/"] = x => {
                return "Default get root";
            };

            Get["/fake/should/have/conflicting/route/defined"] = x => {
                return new Response { Contents = "FakeNancyModuleWithoutBasePath" };
            };

            Post["/"] = x => {
                return "Default post root";
            };

            Put["/"] = x => {
                return "Default put root";
            };
        }
    }
}