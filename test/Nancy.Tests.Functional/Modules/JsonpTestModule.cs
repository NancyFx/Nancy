namespace Nancy.Tests.Functional.Modules
{
    public class JsonpTestModule : NancyModule
    {
        public JsonpTestModule() : base("/test")
        {
            Get("/string", args => "Normal Response");
            Get("/json", args => this.Response.AsJson(true));
            Get("/{name}", args => this.Response.AsJson(new { args.name }));
        }
    }
}
