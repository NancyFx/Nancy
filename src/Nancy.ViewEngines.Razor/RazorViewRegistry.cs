namespace Nancy.ViewEngines.Razor
{
    using System;
    using System.IO;

    public class RazorViewRegistry : IViewEngineRegistry
    {
        public string Extension
        {
            get { return ".cshtml"; }
        }

        public Func<string, object, Action<Stream>> Executor
        {
            get { return (name, model) => RazorViewEngineExtensions.Razor(null, name, model); }
        }
    }
}