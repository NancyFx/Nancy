namespace Nancy.Testing
{
    using System;
    using System.IO;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Xml.Linq;

    /// <summary>
    /// Defines extensions for the <see cref="BrowserResponse"/> type.
    /// </summary>
    public static class BrowserResponseExtensions
    {
        /// <summary>
        /// Asserts that a redirect to a certain location took place.
        /// </summary>
        /// <param name="response">The <see cref="BrowserResponse"/> that the assert should be made on.</param>
        /// <param name="location">The location that should have been redirected to.</param>
        /// <param name="stringComparer">The string comparer that should be used by the assertion. The default value is <see cref="StringComparison.Ordinal"/>.</param>
        public static void ShouldHaveRedirectedTo(this BrowserResponse response, string location, StringComparison stringComparer = StringComparison.Ordinal)
        {
            var validRedirectStatuses = new[]
            {
                HttpStatusCode.MovedPermanently,
                HttpStatusCode.SeeOther,
                HttpStatusCode.TemporaryRedirect
            };

            if (!validRedirectStatuses.Any(x => x == response.StatusCode))
            {
                throw new AssertException(
                    string.Format("Status code should be one of 'MovedPermanently, SeeOther, TemporaryRedirect', but was {0}.", response.StatusCode));
            }

            if (!response.Headers["Location"].Equals(location, stringComparer))
            {
                throw new AssertException(string.Format("Location should have been: {0}, but was {1}", location, response.Headers["Location"]));
            }
        }

        public static XDocument BodyAsXml(this BrowserResponse response)
        {
            using (var contentsStream = new MemoryStream())
            {
                response.Context.Response.Contents.Invoke(contentsStream);
                contentsStream.Position = 0;
                return XDocument.Load(contentsStream);
            }
        }
    }
}