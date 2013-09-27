using System;

using Nancy;
using Nancy.Authentication.Forms;
using Nancy.Demo.Authentication.Forms.Ajax.Models;
using Nancy.Extensions;

namespace Nancy.Demo.Authentication.Forms.Ajax
{
    public class MainModule : NancyModule
    {
        public MainModule()
        {
            this.Get["/"] = x =>
            {
                var model = new IndexModel();

                if (this.Context.CurrentUser != null) {
                    model.IsLoggedIn = true;
                    model.User = new UserModel(this.Context.CurrentUser.UserName);
                }

                return this.View["index",model];
            };
            
            this.Post["/login"] = x => {
                var userGuid = UserDatabase.ValidateUser((string)this.Request.Form.Username, (string)this.Request.Form.Password);

                if (userGuid == null)
                {
                    return this.Context.GetRedirect("~/login?error=true&username=" + (string)this.Request.Form.Username);
                }

                DateTime? expiry = null;
                if (this.Request.Form.RememberMe.HasValue)
                {
                    expiry = DateTime.Now.AddDays(7);
                }

                return this.LoginAndRedirect(userGuid.Value, expiry);
            };

            this.Get["/logout"] = x => {
                return this.LogoutAndRedirect("~/");
            };
        }
    }
}