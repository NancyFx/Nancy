namespace Nancy
{
    using System;
    using System.IO;

    public interface IViewEngineRegistry
    {
        string Extension { get; }
        Func<string, object, Action<Stream>> Executor { get; }
    }
}