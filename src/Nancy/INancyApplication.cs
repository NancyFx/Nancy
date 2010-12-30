namespace Nancy
{
    using System;    
    using System.IO;

    public interface INancyApplication : INancyModuleLocator
    {
        IModuleActivator Activator { get; }

        Func<string, object, Action<Stream>> DefaultProcessor { get; }

        Func<string, object, Action<Stream>> GetTemplateProcessor(string extension);
    }
}