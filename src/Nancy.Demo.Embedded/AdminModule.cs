namespace Demo.Embedded
{
    using Nancy;

    public class AdminModule : NancyModule
    {
        public AdminModule() : base("/admin")
        {
            this.Get["/"] = _ => this.View["index"];
        }
    }
}