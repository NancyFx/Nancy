namespace Nancy.ViewEngines.Razor
{
    using System.Collections.Generic;

    public interface ITextResource : IEnumerable<KeyValuePair<string, IDictionary<string, string>>>
    {
        string this[string key, NancyContext context] { get; set; }
    }
}