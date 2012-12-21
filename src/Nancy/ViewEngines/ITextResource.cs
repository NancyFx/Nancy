namespace Nancy.ViewEngines
{
    public interface ITextResource
    {
        string this[string key, NancyContext context] { get; }
    }
}