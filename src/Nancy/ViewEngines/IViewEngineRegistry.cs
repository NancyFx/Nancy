namespace Nancy.ViewEngines
{
    using System;
    using System.IO;

    public interface IViewEngineRegistry
    {
        Action<Stream> Execute<TModel>(string viewTemplate, TModel model);

        string Extension { get; }
    }
}