namespace Nancy.ViewEngines.Razor
{
    using System.IO;

    public interface IRazorViewCompiler
    {
        IView GetCompiledView(TextReader reader);
    }
}