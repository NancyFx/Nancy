namespace Nancy
{
    using System;    
    using System.IO;

    public interface ITemplateEngineSelector
    {
        Func<string, object, Action<Stream>> DefaultProcessor { get; }

        Func<string, object, Action<Stream>> GetTemplateProcessor(string extension);
    }
}