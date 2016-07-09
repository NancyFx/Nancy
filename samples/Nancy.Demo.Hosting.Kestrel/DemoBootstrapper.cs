namespace Nancy.Demo.Hosting.Kestrel
{
    using System;
    
    public class DemoBootstrapper : DefaultNancyBootstrapper
    {
        public DemoBootstrapper()
        {
            
        }
        
        public DemoBootstrapper(AppConfiguration appConfig)
        {
            /*
            We could register appConfig as an instance in the container which can
            be injected into areas that need it or we could create our own INancyEnvironment
            extension and use that.
            */
            Console.WriteLine(appConfig.Smtp.Server);
            Console.WriteLine(appConfig.Smtp.User);
            Console.WriteLine(appConfig.Logging.IncludeScopes);
        }
    }   
}