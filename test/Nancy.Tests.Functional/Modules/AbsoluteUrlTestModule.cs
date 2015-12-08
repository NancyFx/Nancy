namespace Nancy.Tests.Functional.Modules
{
    public class AbsoluteUrlTestModule : NancyModule
    {
        public AbsoluteUrlTestModule()
        {
            Get["/"] = _ => "hi";

            Get["/querystring"] = _ => this.Request.Query.myKey;
        }

    }
}
