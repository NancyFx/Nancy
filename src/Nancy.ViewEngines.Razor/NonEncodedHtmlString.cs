namespace Nancy.ViewEngines.Razor
{
    /// <summary>
    /// An html string that is not encoded.
    /// </summary>
    public class NonEncodedHtmlString : IHtmlString
    {
        /// <summary>
        /// Represents the empty <see cref="NonEncodedHtmlString"/>. This field is readonly.
        /// </summary>
        public static readonly NonEncodedHtmlString Empty = new NonEncodedHtmlString(string.Empty);

        private readonly string value;

        /// <summary>
        /// Initializes a new instance of the <see cref="NonEncodedHtmlString"/> class.
        /// </summary>
        /// <param name="value">The value.</param>
        public NonEncodedHtmlString(string value)
        {
            this.value = value;
        }

        /// <summary>
        /// Returns an HTML-encoded string.
        /// </summary>
        /// <returns>An HTML-encoded string.</returns>
        public string ToHtmlString()
        {
            return value;
        }

        public static implicit operator NonEncodedHtmlString(string value)
        {
            return new NonEncodedHtmlString(value);
        }
    }
}