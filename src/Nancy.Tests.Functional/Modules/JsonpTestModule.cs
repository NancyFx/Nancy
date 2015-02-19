namespace Nancy.Tests.Functional.Modules
{
    public class JsonpTestModule : NancyModule
    {
        public JsonpTestModule() : base("/test")
        {
            Get["/string"] = x => "Normal Response";
            Get["/json"] = x => this.Response.AsJson(true);
            Get["/{name}"] = parameters => this.Response.AsJson(new { parameters.name });
        }
    }
}
