namespace Nancy.Demo.Authentication
{
    public class MainModule : NancyModule
    {
        public MainModule()
        {
            Get("/", args => {
                return View["Index.cshtml"];
            });

            Get("/login", args => {
                return View["Login.cshtml", this.Request.Query.returnUrl];
            });
        }
    }
}