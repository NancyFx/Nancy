namespace Nancy.Tests.Fakes
{
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
                return "FakeNancyModuleWithoutBasePath";
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