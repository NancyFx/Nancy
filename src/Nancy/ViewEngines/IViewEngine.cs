namespace Nancy.ViewEngines
{
    public interface IViewEngine
    {
        ViewResult RenderView<TModel>(string viewTemplate, TModel model);
    }
}