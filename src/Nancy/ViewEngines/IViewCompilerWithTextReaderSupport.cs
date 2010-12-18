namespace Nancy.ViewEngines
{
    using System.IO;

    public interface IViewCompilerWithTextReaderSupport : IViewCompiler
    {
        IView GetCompiledView<TModel>(TextReader textReader);
    }
}