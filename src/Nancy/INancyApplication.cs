namespace Nancy
{
    using System;    
    using System.IO;

    public interface INancyApplication : INancyModuleLocator
    {
        Func<string, object, Action<Stream>> GetTemplateProcessor(string extension);
        Func<string, object, Action<Stream>> DefaultProcessor { get; }

        IModuleActivator Activator { get; }
    }
}