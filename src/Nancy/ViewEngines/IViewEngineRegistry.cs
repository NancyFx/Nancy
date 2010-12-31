namespace Nancy.ViewEngines
{
    using System;
    using System.IO;

    public interface IViewEngineRegistry
    {
        Func<string, object, Action<Stream>> Executor { get; }

        string Extension { get; }
    }
}