namespace Nancy.ViewEngines.NHaml
{
    public interface INHamlViewCompiler
    {
        IView GetCompiledView<TModel>(string fullPath);
    }
}