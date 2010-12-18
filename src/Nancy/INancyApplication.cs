namespace Nancy
{
    using System;
    using System.IO;

    public interface INancyApplication
    {
        Func<string, object, Action<Stream>> GetTemplateProcessor(string extension);
        Func<string, object, Action<Stream>> DefaultProcessor { get; }
    }
}