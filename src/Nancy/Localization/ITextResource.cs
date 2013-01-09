namespace Nancy.Localization
{
    /// <summary>
    /// Used to return string values
    /// </summary>
    public interface ITextResource
    {
        string this[string key, NancyContext context] { get; }
    }
}