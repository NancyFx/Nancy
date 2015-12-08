namespace Nancy.Demo.Authentication.Stateless
{
    using Nancy.Authentication.Stateless;
    using Nancy.Bootstrapper;
    using Nancy.TinyIoc;

    public class StatelessAuthBootstrapper : DefaultNancyBootstrapper
    {
        protected override void RequestStartup(TinyIoCContainer requestContainer, IPipelines pipelines, NancyContext context)
        {
            // At request startup we modify the request pipelines to
            // include stateless authentication
            //
            // Configuring stateless authentication is simple. Just use the
            // NancyContext to get the apiKey. Then, use the apiKey to get
            // your user's identity.
            var configuration =
                new StatelessAuthenticationConfiguration(nancyContext =>
                    {
                        //for now, we will pull the apiKey from the querystring,
                        //but you can pull it from any part of the NancyContext
                        var apiKey = (string) nancyContext.Request.Query.ApiKey.Value;

                        //get the user identity however you choose to (for now, using a static class/method)
                        return UserDatabase.GetUserFromApiKey(apiKey);
                    });

            AllowAccessToConsumingSite(pipelines);

            StatelessAuthentication.Enable(pipelines, configuration);
        }

        static void AllowAccessToConsumingSite(IPipelines pipelines)
        {
            pipelines.AfterRequest.AddItemToEndOfPipeline(x =>
                {
                    x.Response.Headers.Add("Access-Control-Allow-Origin", "*");
                    x.Response.Headers.Add("Access-Control-Allow-Methods", "POST,GET,DELETE,PUT,OPTIONS");
                });
        }
    }
}