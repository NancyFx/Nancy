using System.Collections.Generic;

namespace Nancy.ViewEngines.Razor
{
    using System.Globalization;

    public interface ITextResource : IEnumerable<KeyValuePair<string, IDictionary<string, string>>>
    {
        string this[string key, string culture] { get; set; }
    }
}