namespace Nancy.ViewEngines.Razor
{
    using Nancy.Helpers;

    /// <summary>
    /// An html string that is encoded.
    /// </summary>
    public class EncodedHtmlString : IHtmlString
    {
        /// <summary>
        /// Represents the empty <see cref="EncodedHtmlString"/>. This field is readonly.
        /// </summary>
        public static readonly EncodedHtmlString Empty = new EncodedHtmlString(string.Empty);

        private readonly string encodedValue;

        /// <summary>
        /// Initializes a new instance of the <see cref="EncodedHtmlString"/> class.
        /// </summary>
        /// <param name="value">The encoded value.</param>
        public EncodedHtmlString(string value)
        {
            this.encodedValue = HttpUtility.HtmlEncode(value);
        }

        /// <summary>
        /// Returns an HTML-encoded string.
        /// </summary>
        /// <returns>An HTML-encoded string.</returns>
        public string ToHtmlString()
        {
            return this.encodedValue;
        }

        public static implicit operator EncodedHtmlString(string value)
        {
            return new EncodedHtmlString(value);
        }

        public static implicit operator string(EncodedHtmlString encoded)
        {
            return encoded.encodedValue;
        }
    }
}
