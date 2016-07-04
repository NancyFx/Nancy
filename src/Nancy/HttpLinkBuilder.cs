namespace Nancy
{
    using System;
    using System.Collections.Generic;
    using System.Text;

    /// <summary>
    /// Builds an RFC 5988 Link HTTP header as a <see cref="List{T}" /> of <see cref="HttpLink" /> objects.
    /// </summary>
    /// <seealso cref="HttpLink" />
    public class HttpLinkBuilder : List<HttpLink>
    {
        private readonly List<string> additionalLinks;

        /// <summary>
        /// Initializes a new instance of the <see cref="HttpLinkBuilder"/> class.
        /// </summary>
        public HttpLinkBuilder()
        {
            this.additionalLinks = new List<string>();
        }

        /// <summary>
        /// Adds the specified link to the builder.
        /// </summary>
        /// <param name="link">The link to add to the builder.</param>
        public void Add(string link)
        {
            if (link == null)
            {
                throw new ArgumentNullException("link");
            }

            this.additionalLinks.Add(link);
        }

        /// <summary>
        /// Returns a <see cref="System.String" /> that represents an RFC 5988 Link HTTP header.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String" /> that represents an RFC 5988 Link HTTP header.
        /// </returns>
        public override string ToString()
        {
            var sb = new StringBuilder();

            foreach (var link in this)
            {
                if (sb.Length > 0)
                {
                    sb.Append(", ");
                }

                sb.Append(link);
            }

            foreach (var link in this.additionalLinks)
            {
                if (sb.Length > 0)
                {
                    sb.Append(", ");
                }

                sb.Append(link);
            }

            return sb.ToString();
        }
    }
}
