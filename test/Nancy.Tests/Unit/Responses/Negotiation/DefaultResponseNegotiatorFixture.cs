namespace Nancy.Tests.Unit.Responses.Negotiation
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Nancy.Responses.Negotiation;

    using Xunit;

    public class DefaultResponseNegotiatorFixture
    {
        [Fact]
        public void Should_encode_URL_in_Link_response_header()
        {
            // Given
            var negotiator = new DefaultResponseNegotiatorWrapper();
            var requestUrl = new Uri("http://localhost:80/george-å");
            var range = new MediaRange("application/vnd.nancy");
            var linkProcessor = new KeyValuePair<string, MediaRange>("nancy", range);

            // When
            var linkHeader = negotiator.CreateLinkHeaderWrapper(requestUrl, new[] { linkProcessor }, null);

            // Then
            Assert.DoesNotContain("å", linkHeader, StringComparison.InvariantCultureIgnoreCase);
            Assert.Contains("%C3%A5", linkHeader, StringComparison.InvariantCultureIgnoreCase);
        }

        private class DefaultResponseNegotiatorWrapper : DefaultResponseNegotiator
        {
            public DefaultResponseNegotiatorWrapper()
                : base(Enumerable.Empty<IResponseProcessor>(), null)
            {
            }

            public string CreateLinkHeaderWrapper(Url requestUrl, IEnumerable<KeyValuePair<string, MediaRange>> linkProcessors, string existingLinkHeader)
            {
                return base.CreateLinkHeader(requestUrl, linkProcessors, existingLinkHeader);
            }
        }
    }
}
