namespace Nancy.Testing
{
    using System;

    public static class BrowserResponseExtensions
    {
        public static void ShouldHaveRedirectedTo(this BrowserResponse response, string location, StringComparison stringComparer = StringComparison.InvariantCulture)
        {
            if (response.StatusCode != HttpStatusCode.SeeOther)
            {
                throw new AssertException("Status code should be SeeOther");
            }

            if (!response.Headers["Location"].Equals(location, stringComparer))
            {
                throw new AssertException(String.Format("Location should have been: {0}, but was {1}", location, response.Headers["Location"]));
            }
        }
    }
}