namespace Nancy.Demo.Authentication.Forms
{
    using Nancy.Demo.Authentication.Forms.Models;
    using Nancy.Security;

    public class PartlySecureModule : NancyModule
    {
        public PartlySecureModule()
            : base("/partlysecure")
        {
            Get["/"] = _ => "No auth needed! <a href='partlysecure/secured'>Enter the secure bit!</a>";

            Get["/secured"] = x => {
                this.RequiresAuthentication();

                var model = new UserModel(this.Context.CurrentUser.Identity.Name);
                return View["secure.cshtml", model];
            };
        }
    }
}