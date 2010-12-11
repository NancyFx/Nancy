namespace Nancy.ViewEngines.Razor
{
    using System.IO;

    public interface IViewCompiler
    {
        IView GetCompiledView(TextReader fullPath);
    }
}