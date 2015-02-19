namespace Nancy.Demo.Authentication.Token
{
    using Nancy.Authentication.Token;
    using Nancy.Bootstrapper;
    using Nancy.TinyIoc;

    public class TokenAuthBootstrapper : DefaultNancyBootstrapper
    {
        protected override void ConfigureApplicationContainer(TinyIoCContainer container)
        {
            container.Register<ITokenizer>(new Tokenizer());
            // Example options for specifying additional values for token generation

            //container.Register<ITokenizer>(new Tokenizer(cfg =>
            //                                             cfg.AdditionalItems(
            //                                                 ctx =>
            //                                                 ctx.Request.Headers["X-Custom-Header"].FirstOrDefault(),
            //                                                 ctx => ctx.Request.Query.extraValue)));
        }

        protected override void RequestStartup(TinyIoCContainer container, IPipelines pipelines, NancyContext context)
        {
            TokenAuthentication.Enable(pipelines, new TokenAuthenticationConfiguration(container.Resolve<ITokenizer>()));
        }
    }
}