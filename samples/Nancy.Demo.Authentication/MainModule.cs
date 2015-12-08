namespace Nancy.Demo.Authentication
{
    public class MainModule : LegacyNancyModule
    {
        public MainModule()
        {
            Get["/"] = x => {
                return View["Index.cshtml"];
            };

            Get["/login"] = x => {
                return View["Login.cshtml", this.Request.Query.returnUrl];
            };
        }
    }
}