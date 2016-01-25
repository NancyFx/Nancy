namespace Nancy.Tests.Fakes
{
    public class FakeNancyModuleWithoutBasePath : LegacyNancyModule
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

            Get["/greet/{name}"] = x =>
            {
                return string.Concat("Hello ", x.name);
            };

            Get["/filtered", req => false] = x =>
            {
                return "I should never be run because I am filtered";
            };

            Get["/notfiltered", req => true] = x =>
            {
                return "I should always be fine because my filter returns true";
            };

            Post["/"] = x => {
                return "Default post root";
            };

            Put["/"] = x => {
                return "Default put root";
            };

            Get["/filt", req => false] = x =>
            {
                return "false";
            };

            Get["/filt", req => true] = x =>
            {
                return "true";
            };

            Get["/filt", req => false] = x =>
            {
                return "false";
            };
        }
    }
}
