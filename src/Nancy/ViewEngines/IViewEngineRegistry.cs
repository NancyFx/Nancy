namespace Nancy.ViewEngines
{
    public interface IViewEngineRegistry
    {
        IViewEngine ViewEngine { get; }

        string Extension { get; }
    }
}