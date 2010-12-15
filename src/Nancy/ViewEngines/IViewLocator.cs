namespace Nancy.ViewEngines
{
    // Notice that the template might not have a "Path".
    // For example, it could be embedded. So that's why this 
    // returns a reader.
    public interface IViewLocator
    {
        ViewLocationResult GetTemplateContents(string viewTemplate);
    }
}