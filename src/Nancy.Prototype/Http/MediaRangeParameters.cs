namespace Nancy.Prototype.Http
{
    using System.Collections;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;

    [DebuggerDisplay("{ToString(), nq}")]
    public partial struct MediaRangeParameters : IReadOnlyDictionary<string, string>
    {
        private static readonly char[] ParameterSeparator = { ';' };

        private static readonly char[] KeyValueSeparator = { '=' };

        private readonly IReadOnlyDictionary<string, string> parameters;

        public MediaRangeParameters(IReadOnlyDictionary<string, string> parameters)
        {
            Check.NotNull(parameters, nameof(parameters));

            this.parameters = parameters;
        }

        public int Count => this.parameters.Count;

        public bool IsEmpty => this.Count == 0;

        public IEnumerable<string> Keys => this.parameters.Keys;

        public IEnumerable<string> Values => this.parameters.Values;

        public string this[string key] => this.parameters[key];

        public IEnumerator<KeyValuePair<string, string>> GetEnumerator() => this.parameters.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();

        public bool ContainsKey(string key) => this.parameters.ContainsKey(key);

        public bool TryGetValue(string key, out string value) => this.parameters.TryGetValue(key, out value);

        public bool Matches(MediaRangeParameters other)
        {
            var ordered = this.parameters.OrderBy(x => x.Key);
            var otherOrdered = other.parameters.OrderBy(x => x.Key);

            return ordered.SequenceEqual(otherOrdered);
        }

        public override string ToString()
        {
            return string.Join("; ", this.parameters.Select(x => string.Concat(x.Key, "=", x.Value)));
        }
    }
}
