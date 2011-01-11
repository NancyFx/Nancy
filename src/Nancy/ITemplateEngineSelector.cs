namespace Nancy
{
    using System;    
    using System.IO;

    // TODO - Should we add some content negotiation here, or prior to this,
    // for checking accept headers? Need to know the content type the 
    // template processors spit out.
    public interface ITemplateEngineSelector
    {
        Func<string, object, Action<Stream>> DefaultProcessor { get; }

        Func<string, object, Action<Stream>> GetTemplateProcessor(string extension);
    }
}