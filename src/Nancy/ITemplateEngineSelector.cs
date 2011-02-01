namespace Nancy
{
    using ViewEngines;

    public interface ITemplateEngineSelector
    {
        IViewEngine DefaultProcessor { get; }
        IViewEngine GetTemplateProcessor(string extension);
    }
}