namespace Nancy.Testing
{
    using System;
    using System.IO;
    using HtmlAgilityPack;
    using HtmlAgilityPlus;

    /// <summary>
    /// A basic wrapper around HTML Agility pack documents and
    /// sharp query
    /// </summary>
    public class DocumentWrapper
    {
        private enum SourceType
        {
            Stream,
            String,
        }

        private SourceType sourceType;

        private Stream inputStream;

        private string inputString;

        private HtmlDocument agilityPackDocumentInternal;

        private SharpQuery sharpQueryInternal;

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

        /// <summary>
        /// Initializes a new instance of the <see cref="DocumentWrapper"/> class.
        /// </summary>
        /// <param name="inputStream">
        /// The input stream.
        /// </param>
        public DocumentWrapper(Stream inputStream)
        {
            if (inputStream == null)
            {
                throw new ArgumentNullException("inputStream");
            }

            this.inputStream = inputStream;
            this.sourceType = SourceType.Stream;

            // The context extension handles the deferred loading side, so
            // pull in the stream contents straight away.
            this.LoadDocument();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DocumentWrapper"/> class.
        /// </summary>
        /// <param name="inputString">
        /// The input html string.
        /// </param>
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
        /// <param name="selector">CSS3 selector</param>
        /// <returns>QueryWrapper instance</returns>
        public QueryWrapper this[string selector]
        {
            get
            {
                return this.QueryEngine.Find(selector);
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
            using (var reader = new StreamReader(this.inputStream))
            {
                this.agilityPackDocumentInternal = new HtmlDocument();
                var htmlContents = reader.ReadToEnd();
                this.agilityPackDocumentInternal.LoadHtml(htmlContents);
            }
        }
    }
}