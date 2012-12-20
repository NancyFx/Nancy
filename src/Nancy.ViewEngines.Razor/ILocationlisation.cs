using System.Collections.Generic;

namespace Nancy.ViewEngines.Razor
{
    using System.Globalization;

    public interface ILocationlisation : IEnumerable<KeyValuePair<string, string>>
    {
        string this[string key] { get; set; }
        CultureInfo CurrentCulture { get; set; }
    }
}