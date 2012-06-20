using Nancy.Demo.Authentication.Stateless.Models;
using Nancy.Security;

namespace Nancy.Demo.Authentication.Stateless
{
    public class SecureModule : NancyModule
    {
        //by this time, the api key should have already been pulled out of our querystring
        //and, using the api key, an identity assigned to our NancyContext
        public SecureModule() : base("/secure")
        {
            this.RequiresAuthentication();

            Get["/"] = x =>
                {
                    //Context.CurrentUser was set by StatelessAuthentication earlier in the pipeline
                    var identity = (DemoUserIdentity)Context.CurrentUser;

                    //return the secure information in a json response
                    var userModel = new UserModel(identity.UserName);
                    return Response.AsJson(userModel);
                };
        }
    }
}