namespace Nancy.ViewEngines
{
    public interface IViewCompiler
    {
        IView GetCompiledView<TModel>(IViewLocationResult viewLocationResult);
    }
}