namespace Nancy.Testing
{
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using AngleSharp.Dom.Html;
    using AngleSharp.Parser.Html;

    /// <summary>
    /// A basic wrapper around CsQuery
    /// </summary>
    public class DocumentWrapper
    {
        private readonly IHtmlDocument document;

        /// <summary>
        /// Initializes a new instance of the <see cref="DocumentWrapper"/> class.
        /// </summary>
        /// <param name="buffer">The document represented as a byte array.</param>
        public DocumentWrapper(IEnumerable<byte> buffer)
        {
            var parser = new HtmlParser();
            using (var stream = new MemoryStream(buffer.ToArray()))
            {
                this.document = parser.Parse(stream);
            }
        }

        /// <summary>
        /// Gets elements from CSS3 selectors
        /// </summary>
        /// <param name="selector">The CSS3 selector that should be applied.</param>
        /// <returns>A <see cref="QueryWrapper"/> instance.</returns>
        public QueryWrapper this[string selector]
        {
            get { return new QueryWrapper(this.document.QuerySelectorAll(selector).ToArray()); }
        }
    }
}
