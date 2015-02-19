namespace Nancy.Demo.Authentication.Token
{
    using Nancy.Authentication.Token;
    using Nancy.Security;

    public class AuthModule : NancyModule
    {
        public AuthModule(ITokenizer tokenizer)
            : base("/auth")
        {
            Post["/"] = x =>
                {
                    var userName = (string)this.Request.Form.UserName;
                    var password = (string)this.Request.Form.Password;

                    var userIdentity = UserDatabase.ValidateUser(userName, password);

                    if (userIdentity == null)
                    {
                        return HttpStatusCode.Unauthorized;
                    }

                    var token = tokenizer.Tokenize(userIdentity, Context);

                    return new
                        {
                            Token = token,
                        };
                };

            Get["/validation"] = _ =>
                {
                    this.RequiresAuthentication();
                    return "Yay! You are authenticated!";
                };

            Get["/admin"] = _ =>
            {
                this.RequiresClaims(new[] { "admin" });
                return "Yay! You are authorized!";
            };
        }
    }
}