namespace Nancy.Demo.InProcSessions
{
    using System;
    using Nancy.Bootstrapper;
    using Nancy.Session.InProc;
    using Nancy.TinyIoc;

    public class Bootstrapper : DefaultNancyBootstrapper
    {
        protected override void ApplicationStartup(TinyIoCContainer container, IPipelines pipelines)
        {
            StaticConfiguration.DisableErrorTraces = false;

            var sessionConfig = new InProcSessionsConfiguration
            {
                SessionTimeout = TimeSpan.FromMinutes(3),
                CacheTrimInterval = TimeSpan.FromMinutes(10)
            };
            InProcSessions.Enable(pipelines, sessionConfig);

            base.ApplicationStartup(container, pipelines);
        }
    }
}