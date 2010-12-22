namespace Nancy
{
    using System;    
    using System.IO;
    using Sessions;

    public interface INancyApplication : INancyModuleLocator
    {
        Func<string, object, Action<Stream>> GetTemplateProcessor(string extension);
        Func<string, object, Action<Stream>> DefaultProcessor { get; }
        IModuleActivator Activator { get; }
        ISessionStore SessionStore { get; }
    }
}