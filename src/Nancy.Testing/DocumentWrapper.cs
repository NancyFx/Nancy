namespace Nancy.Testing
{
    using System;
    using System.IO;
    using HtmlAgilityPack;
    using HtmlAgilityPlus;

    /// <summary>
    /// A basic wrapper around HTML Agility pack documents and sharp query
    /// </summary>
    public class DocumentWrapper
    {
        private enum SourceType
        {
            Stream,
            String,
        }

        private readonly SourceType sourceType;
        private readonly Stream stream;
        private readonly string inputString;
        private HtmlDocument agilityPackDocumentInternal;
        private SharpQuery sharpQueryInternal;

        /// <summary>
        /// Initializes a new instance of the <see cref="DocumentWrapper"/> class.
        /// </summary>
        /// <param name="stream">The HTTP response stream that should be wrapped.</param>
        public DocumentWrapper(Stream stream)
        {
            if (stream == null)
            {
                throw new ArgumentNullException("stream");
            }

            this.stream = stream;
            this.sourceType = SourceType.Stream;

            // The context extension handles the deferred loading side, so
            // pull in the stream contents straight away.
            this.LoadDocument();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DocumentWrapper"/> class.
        /// </summary>
        /// <param name="inputString">The html that should be wrapped.</param>
        public DocumentWrapper(string inputString)
        {
            if (inputString == null)
            {
                throw new ArgumentNullException("inputString");
            }

            this.inputString = inputString;
            this.sourceType = SourceType.String;
        }

        /// <summary>
        /// Gets elements from CSS3 selectors
        /// </summary>
        /// <param name="selector">The CSS3 selector that should be applied.</param>
        /// <returns>A <see cref="QueryWrapper"/> instance.</returns>
        public QueryWrapper this[string selector]
        {
            get
            {
                return this.QueryEngine.Find(selector);
            }
        }

        private SharpQuery QueryEngine
        {
            get
            {
                if (this.sharpQueryInternal == null)
                {
                    this.LoadDocument();
                }

                return this.sharpQueryInternal;
            }
        }

        private void LoadDocument()
        {
            this.agilityPackDocumentInternal = new HtmlDocument();

            switch (this.sourceType)
            {
                case SourceType.Stream:
                    this.LoadDocumentFromStream();
                    break;
                case SourceType.String:
                    this.LoadDocumentFromString();
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            this.sharpQueryInternal = new SharpQuery(this.agilityPackDocumentInternal);
        }

        private void LoadDocumentFromString()
        {
            this.agilityPackDocumentInternal.LoadHtml(this.inputString);
        }

        private void LoadDocumentFromStream()
        {
            using (var reader = new StreamReader(this.stream))
            {
                this.agilityPackDocumentInternal = new HtmlDocument();
                var htmlContents = reader.ReadToEnd();
                this.agilityPackDocumentInternal.LoadHtml(htmlContents);
            }
        }
    }
}