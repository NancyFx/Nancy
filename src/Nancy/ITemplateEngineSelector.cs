namespace Nancy
{
    using System;    
    using System.IO;

    public interface ITemplateEngineSelector
    {
        Func<string, TModel, Action<Stream>> DefaultProcessor<TModel>();
        Func<string, TModel, Action<Stream>> GetTemplateProcessor<TModel>(string extension);
    }
}