namespace Nancy.Demo.Authentication.Stateless
{
    using System;
    using Nancy.Demo.Authentication.Stateless.Models;
    using Nancy.Security;

    public class SecureModule : NancyModule
    {
        //by this time, the api key should have already been pulled out of our querystring
        //and, using the api key, an identity assigned to our NancyContext
        public SecureModule()
        {
            this.RequiresAuthentication();

            Get("secure", args =>
            {
                //Context.CurrentUser was set by StatelessAuthentication earlier in the pipeline
                var identity = this.Context.CurrentUser;

                //return the secure information in a json response
                var userModel = new UserModel(identity.Identity.Name);
                return this.Response.AsJson(new
                    {
                        SecureContent = "here's some secure content that you can only see if you provide a correct apiKey",
                        User = userModel
                    });
            });

            Post("secure/create_user", args =>
            {
                Tuple<string, string> user = UserDatabase.CreateUser(this.Context.Request.Form["username"], this.Context.Request.Form["password"]);
                return this.Response.AsJson(new { username = user.Item1 });
            });
        }
    }
}
