namespace Nancy
{
    using System;    
    using System.IO;
    using Sessions;

    public interface INancyApplication : INancyModuleLocator
    {
        IModuleActivator Activator { get; }

        Func<string, object, Action<Stream>> DefaultProcessor { get; }

		Func<string, object, Action<Stream>> GetTemplateProcessor(string extension);
		
		ISessionStore SessionStore { get; }
    }
}