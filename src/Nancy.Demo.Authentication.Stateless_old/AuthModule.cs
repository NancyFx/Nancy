namespace Nancy.Demo.Authentication.Stateless
{
    public class AuthModule : NancyModule
    {
        public AuthModule() : base("/auth/")
        {
            //the Post["/login"] method is used mainly to fetch the api key for subsequent calls
            Post["/"] = x =>
                {
                    string apiKey = UserDatabase.ValidateUser((string) Request.Form.Username,
                                                              (string) Request.Form.Password);

                    return string.IsNullOrEmpty(apiKey)
                               ? new Response {StatusCode = HttpStatusCode.Unauthorized}
                               : Response.AsJson(new {ApiKey = apiKey});
                };

            //do something to destroy the api key, maybe?                    
            Delete["/"] = x => { return null; };
        }
    }
}