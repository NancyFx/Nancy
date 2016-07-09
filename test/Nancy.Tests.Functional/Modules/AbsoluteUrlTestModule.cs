namespace Nancy.Tests.Functional.Modules
{
    public class AbsoluteUrlTestModule : NancyModule
    {
        public AbsoluteUrlTestModule()
        {
            Get("/", args => "hi");

            Get("/querystring", args => this.Request.Query.myKey);
        }
    }
}
