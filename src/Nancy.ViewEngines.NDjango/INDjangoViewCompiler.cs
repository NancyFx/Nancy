namespace Nancy.ViewEngines.NDjango
{
    public interface INDjangoViewCompiler
    {
        IView GetCompiledView(string fullPath);
    }
}