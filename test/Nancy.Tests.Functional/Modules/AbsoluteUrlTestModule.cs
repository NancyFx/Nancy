namespace Nancy.Tests.Functional.Modules
{
    public class AbsoluteUrlTestModule : LegacyNancyModule
    {
        public AbsoluteUrlTestModule()
        {
            Get["/"] = _ => "hi";

            Get["/querystring"] = _ => this.Request.Query.myKey;
        }

    }
}
