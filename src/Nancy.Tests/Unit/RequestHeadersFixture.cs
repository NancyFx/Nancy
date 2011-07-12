namespace Nancy.Tests.Unit
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;

    using Nancy;
    using FakeItEasy;
    using Xunit;
    using Xunit.Extensions;

    public class RequestHeadersFixture
    {
        public RequestHeadersFixture()
        {
        }

        [Fact]
        public void Should_return_empty_enumerable_when_accept_headers_are_not_available()
        {
            // Given
            var values = new Dictionary<string, IEnumerable<string>>();

            // When
            var headers = new RequestHeaders(values);
            
            // Then
            headers.Accept.ShouldHaveCount(0);
        }

        [Fact]
        public void Should_return_accept_headers_when_available()
        {
            // Given
            var acceptValues = new[] { "text/plain", "text/ninja" };
            var values = new Dictionary<string, IEnumerable<string>> { { "Accept", acceptValues } };

            // When
            var headers = new RequestHeaders(values);

            // Then
            headers.Accept.ShouldBeSameAs(acceptValues);
        }

        [Theory]
        [InlineData("accept")]
        [InlineData("AcCepT")]
        public void Should_ignore_case_of_accept_header_name_when_retrieving_values(string headerName)
        {
            // Given
            var acceptValues = new[] { "text/plain", "text/ninja" };
            var values = new Dictionary<string, IEnumerable<string>> { { headerName, acceptValues } };

            // When
            var headers = new RequestHeaders(values);

            // Then
            headers.Accept.ShouldBeSameAs(acceptValues);
        }

        [Fact]
        public void Should_return_empty_enumerable_when_accept_charset_headers_are_not_available()
        {
            // Given
            var values = new Dictionary<string, IEnumerable<string>>();

            // When
            var headers = new RequestHeaders(values);

            // Then
            headers.AcceptCharset.ShouldHaveCount(0);
        }

        [Fact]
        public void Should_return_accept_charset_headers_when_available()
        {
            // Given
            var acceptValues = new[] { "utf-8", "iso-8859-5" };
            var values = new Dictionary<string, IEnumerable<string>> { { "Accept-Charset", acceptValues } };

            // When
            var headers = new RequestHeaders(values);

            // Then
            headers.AcceptCharset.ShouldBeSameAs(acceptValues);
        }

        [Theory]
        [InlineData("accept-charset")]
        [InlineData("AcCepT-cHaRsET")]
        public void Should_ignore_case_of_accept_charset_header_name_when_retrieving_values(string headerName)
        {
            // Given
            var acceptValues = new[] { "utf-8", "iso-8859-5" };
            var values = new Dictionary<string, IEnumerable<string>> { { headerName, acceptValues } };

            // When
            var headers = new RequestHeaders(values);

            // Then
            headers.AcceptCharset.ShouldBeSameAs(acceptValues);
        }

        [Fact]
        public void Should_return_empty_enumerable_when_accept_encoding_headers_are_not_available()
        {
            // Given
            var values = new Dictionary<string, IEnumerable<string>>();

            // When
            var headers = new RequestHeaders(values);

            // Then
            headers.AcceptEncoding.ShouldHaveCount(0);
        }

        [Fact]
        public void Should_return_accept_encoding_headers_when_available()
        {
            // Given
            var acceptValues = new[] { "compress", "sdch" };
            var values = new Dictionary<string, IEnumerable<string>> { { "Accept-Encoding", acceptValues } };

            // When
            var headers = new RequestHeaders(values);

            // Then
            headers.AcceptEncoding.ShouldBeSameAs(acceptValues);
        }

        [Theory]
        [InlineData("accept-encoding")]
        [InlineData("AcCepT-ENcOdinG")]
        public void Should_ignore_case_of_accept_encoding_header_name_when_retrieving_values(string headerName)
        {
            // Given
            var acceptValues = new[] { "compress", "sdch" };
            var values = new Dictionary<string, IEnumerable<string>> { { headerName, acceptValues } };

            // When
            var headers = new RequestHeaders(values);

            // Then
            headers.AcceptEncoding.ShouldBeSameAs(acceptValues);
        }

        [Fact]
        public void Should_return_empty_enumerable_when_accept_language_headers_are_not_available()
        {
            // Given
            var values = new Dictionary<string, IEnumerable<string>>();

            // When
            var headers = new RequestHeaders(values);

            // Then
            headers.AcceptLanguage.ShouldHaveCount(0);
        }

        [Fact]
        public void Should_return_accept_language_headers_when_available()
        {
            // Given
            var acceptValues = new[] { "en-US", "sv-SE" };
            var values = new Dictionary<string, IEnumerable<string>> { { "Accept-Language", acceptValues } };

            // When
            var headers = new RequestHeaders(values);

            // Then
            headers.AcceptLanguage.ShouldBeSameAs(acceptValues);
        }

        [Theory]
        [InlineData("accept-language")]
        [InlineData("AcCepT-LaNGUage")]
        public void Should_ignore_case_of_accept_language_header_name_when_retrieving_values(string headerName)
        {
            // Given
            var acceptValues = new[] { "en-US", "sv-SE" };
            var values = new Dictionary<string, IEnumerable<string>> { { headerName, acceptValues } };

            // When
            var headers = new RequestHeaders(values);

            // Then
            headers.AcceptLanguage.ShouldBeSameAs(acceptValues);
        }

        [Fact]
        public void Should_return_empty_string_when_authorization_headers_are_not_available()
        {
            // Given
            var values = new Dictionary<string, IEnumerable<string>>();

            // When
            var headers = new RequestHeaders(values);

            // Then
            headers.Authorization.ShouldBeEmpty();
        }

        [Fact]
        public void Should_return_authorization_headers_when_available()
        {
            // Given
            var acceptValues = new[] { "Basic QWxhZGRpbjpvcGVuIHNlc2FtZQ==" };
            var values = new Dictionary<string, IEnumerable<string>> { { "Authorization", acceptValues } };

            // When
            var headers = new RequestHeaders(values);

            // Then
            headers.Authorization.ShouldBeSameAs(acceptValues[0]);
        }

        [Theory]
        [InlineData("authorization")]
        [InlineData("AutHORizaTion")]
        public void Should_ignore_case_of_authorization_header_name_when_retrieving_values(string headerName)
        {
            // Given
            var acceptValues = new[] { "Basic QWxhZGRpbjpvcGVuIHNlc2FtZQ==" };
            var values = new Dictionary<string, IEnumerable<string>> { { headerName, acceptValues } };

            // When
            var headers = new RequestHeaders(values);

            // Then
            headers.Authorization.ShouldBeSameAs(acceptValues[0]);
        }

        [Fact]
        public void Should_return_empty_enumerable_when_cache_control_headers_are_not_available()
        {
            // Given
            var values = new Dictionary<string, IEnumerable<string>>();

            // When
            var headers = new RequestHeaders(values);

            // Then
            headers.CacheControl.ShouldHaveCount(0);
        }

        [Fact]
        public void Should_return_cache_control_headers_when_available()
        {
            // Given
            var acceptValues = new[] { "public", "max-age=123445" };
            var values = new Dictionary<string, IEnumerable<string>> { { "Cache-Control", acceptValues } };

            // When
            var headers = new RequestHeaders(values);

            // Then
            headers.CacheControl.ShouldBeSameAs(acceptValues);
        }

        [Theory]
        [InlineData("cache-control")]
        [InlineData("CaCHe-ContROL")]
        public void Should_ignore_case_of_cache_control_header_name_when_retrieving_values(string headerName)
        {
            // Given
            var acceptValues = new[] { "public", "max-age=123445" };
            var values = new Dictionary<string, IEnumerable<string>> { { headerName, acceptValues } };

            // When
            var headers = new RequestHeaders(values);

            // Then
            headers.CacheControl.ShouldBeSameAs(acceptValues);
        }

        [Fact]
        public void Should_return_empty_string_when_connection_headers_are_not_available()
        {
            // Given
            var values = new Dictionary<string, IEnumerable<string>>();

            // When
            var headers = new RequestHeaders(values);

            // Then
            headers.Connection.ShouldBeEmpty();
        }

        [Fact]
        public void Should_return_connection_headers_when_available()
        {
            // Given
            var acceptValues = new[] { "closed" };
            var values = new Dictionary<string, IEnumerable<string>> { { "Connection", acceptValues } };

            // When
            var headers = new RequestHeaders(values);

            // Then
            headers.Connection.ShouldBeSameAs(acceptValues[0]);
        }

        [Theory]
        [InlineData("connection")]
        [InlineData("CONNecTION")]
        public void Should_ignore_case_of_connection_header_name_when_retrieving_values(string headerName)
        {
            // Given
            var acceptValues = new[] { "closed" };
            var values = new Dictionary<string, IEnumerable<string>> { { headerName, acceptValues } };

            // When
            var headers = new RequestHeaders(values);

            // Then
            headers.Connection.ShouldBeSameAs(acceptValues[0]);
        }

        [Fact]
        public void Should_return_zero_when_content_length_headers_are_not_available()
        {
            // Given
            var values = new Dictionary<string, IEnumerable<string>>();

            // When
            var headers = new RequestHeaders(values);

            // Then
            headers.ContentLength.ShouldEqual(0L);
        }

        [Fact]
        public void Should_return_content_length_headers_when_available()
        {
            // Given
            var acceptValues = new[] { "12345" };
            var values = new Dictionary<string, IEnumerable<string>> { { "Content-Length", acceptValues } };

            // When
            var headers = new RequestHeaders(values);

            // Then
            headers.ContentLength.ShouldEqual(12345L);
        }

        [Theory]
        [InlineData("content-length")]
        [InlineData("CoNTEnt-LENGth")]
        public void Should_ignore_case_of_content_length_header_name_when_retrieving_values(string headerName)
        {
            // Given
            var acceptValues = new[] { "12345" };
            var values = new Dictionary<string, IEnumerable<string>> { { headerName, acceptValues } };

            // When
            var headers = new RequestHeaders(values);

            // Then
            headers.ContentLength.ShouldEqual(12345L);
        }

        [Fact]
        public void Should_return_empty_string_when_content_type_headers_are_not_available()
        {
            // Given
            var values = new Dictionary<string, IEnumerable<string>>();

            // When
            var headers = new RequestHeaders(values);

            // Then
            headers.ContentType.ShouldBeEmpty();
        }

        [Fact]
        public void Should_return_content_type_header_when_available()
        {
            // Given
            var acceptValues = new[] { "text/ninja" };
            var values = new Dictionary<string, IEnumerable<string>> { { "Content-Type", acceptValues } };

            // When
            var headers = new RequestHeaders(values);

            // Then
            headers.ContentType.ShouldEqual("text/ninja");
        }

        [Theory]
        [InlineData("content-type")]
        [InlineData("CoNTEnt-tYPe")]
        public void Should_ignore_case_of_content_type_header_name_when_retrieving_values(string headerName)
        {
            // Given
            var acceptValues = new[] { "text/ninja" };
            var values = new Dictionary<string, IEnumerable<string>> { { headerName, acceptValues } };

            // When
            var headers = new RequestHeaders(values);

            // Then
            headers.ContentType.ShouldEqual("text/ninja");
        }

        [Fact]
        public void Should_return_min_date_when_date_headers_are_not_available()
        {
            // Given
            var values = new Dictionary<string, IEnumerable<string>>();

            // When
            var headers = new RequestHeaders(values);

            // Then
            headers.Date.ShouldEqual(DateTime.MinValue);
        }

        [Fact]
        public void Should_return_date_when_available()
        {
            // Given
            var expectedDate = new DateTime(2011, 11, 15, 8, 12, 31);
            var acceptValues = new[] { "Tue, 15 Nov 2011 08:12:31 GMT" };
            var values = new Dictionary<string, IEnumerable<string>> { { "Date", acceptValues } };

            // When
            var headers = new RequestHeaders(values);

            // Then
            headers.Date.ShouldEqual(expectedDate);
        }

        [Theory]
        [InlineData("date")]
        [InlineData("daTE")]
        public void Should_ignore_case_of_date_header_name_when_retrieving_values(string headerName)
        {
            // Given
            var expectedDate = new DateTime(2011, 11, 15, 8, 12, 31);
            var acceptValues = new[] { "Tue, 15 Nov 2011 08:12:31 GMT" };
            var values = new Dictionary<string, IEnumerable<string>> { { headerName, acceptValues } };

            // When
            var headers = new RequestHeaders(values);

            // Then
            headers.Date.ShouldEqual(expectedDate);
        }

        [Fact]
        public void Should_return_empty_string_when_host_headers_are_not_available()
        {
            // Given
            var values = new Dictionary<string, IEnumerable<string>>();

            // When
            var headers = new RequestHeaders(values);

            // Then
            headers.Host.ShouldBeEmpty();
        }

        [Fact]
        public void Should_return_host_headers_when_available()
        {
            // Given
            var acceptValues = new[] { "en.wikipedia.org" };
            var values = new Dictionary<string, IEnumerable<string>> { { "Host", acceptValues } };

            // When
            var headers = new RequestHeaders(values);

            // Then
            headers.Host.ShouldBeSameAs(acceptValues[0]);
        }

        [Theory]
        [InlineData("host")]
        [InlineData("hOsT")]
        public void Should_ignore_case_of_host_header_name_when_retrieving_values(string headerName)
        {
            // Given
            var acceptValues = new[] { "en.wikipedia.org" };
            var values = new Dictionary<string, IEnumerable<string>> { { headerName, acceptValues } };

            // When
            var headers = new RequestHeaders(values);

            // Then
            headers.Host.ShouldBeSameAs(acceptValues[0]);
        }

        [Fact]
        public void Should_return_empty_enumerable_when_ifmatch_headers_are_not_available()
        {
            // Given
            var values = new Dictionary<string, IEnumerable<string>>();

            // When
            var headers = new RequestHeaders(values);

            // Then
            headers.IfMatch.ShouldHaveCount(0);
        }

        [Fact]
        public void Should_return_ifmatch_headers_when_available()
        {
            // Given
            var acceptValues = new[] { "xyzzy", "c3piozzzz" };
            var values = new Dictionary<string, IEnumerable<string>> { { "If-Match", acceptValues } };

            // When
            var headers = new RequestHeaders(values);

            // Then
            headers.IfMatch.ShouldBeSameAs(acceptValues);
        }

        [Theory]
        [InlineData("if-match")]
        [InlineData("If-MaTCH")]
        public void Should_ignore_case_of_ifmatch_header_name_when_retrieving_values(string headerName)
        {
            // Given
            var acceptValues = new[] { "xyzzy", "c3piozzzz" };
            var values = new Dictionary<string, IEnumerable<string>> { { headerName, acceptValues } };

            // When
            var headers = new RequestHeaders(values);

            // Then
            headers.IfMatch.ShouldBeSameAs(acceptValues);
        }

        [Fact]
        public void Should_return_min_date_when_ifmodifiedsince_headers_are_not_available()
        {
            // Given
            var values = new Dictionary<string, IEnumerable<string>>();

            // When
            var headers = new RequestHeaders(values);

            // Then
            headers.IfModifiedSince.ShouldEqual(DateTime.MinValue);
        }

        [Fact]
        public void Should_return_ifmodifiedsince_when_available()
        {
            // Given
            var expectedDate = new DateTime(2011, 11, 15, 8, 12, 31);
            var acceptValues = new[] { "Tue, 15 Nov 2011 08:12:31 GMT" };
            var values = new Dictionary<string, IEnumerable<string>> { { "If-Modified-Since", acceptValues } };

            // When
            var headers = new RequestHeaders(values);

            // Then
            headers.IfModifiedSince.ShouldEqual(expectedDate);
        }

        [Theory]
        [InlineData("if-modified-since")]
        [InlineData("IF-MODIFIED-SINCE")]
        public void Should_ignore_case_of_ifmodifiedsince_header_name_when_retrieving_values(string headerName)
        {
            // Given
            var expectedDate = new DateTime(2011, 11, 15, 8, 12, 31);
            var acceptValues = new[] { "Tue, 15 Nov 2011 08:12:31 GMT" };
            var values = new Dictionary<string, IEnumerable<string>> { { headerName, acceptValues } };

            // When
            var headers = new RequestHeaders(values);

            // Then
            headers.IfModifiedSince.ShouldEqual(expectedDate);
        }

        [Fact]
        public void Should_return_empty_enumerable_when_ifnonematch_headers_are_not_available()
        {
            // Given
            var values = new Dictionary<string, IEnumerable<string>>();

            // When
            var headers = new RequestHeaders(values);

            // Then
            headers.IfNoneMatch.ShouldHaveCount(0);
        }

        [Fact]
        public void Should_return_ifnonematch_headers_when_available()
        {
            // Given
            var acceptValues = new[] { "xyzzy", "c3piozzzz" };
            var values = new Dictionary<string, IEnumerable<string>> { { "If-None-Match", acceptValues } };

            // When
            var headers = new RequestHeaders(values);

            // Then
            headers.IfNoneMatch.ShouldBeSameAs(acceptValues);
        }

        [Theory]
        [InlineData("if-none-match")]
        [InlineData("If-NONe-MaTCH")]
        public void Should_ignore_case_of_ifnonematch_header_name_when_retrieving_values(string headerName)
        {
            // Given
            var acceptValues = new[] { "xyzzy", "c3piozzzz" };
            var values = new Dictionary<string, IEnumerable<string>> { { headerName, acceptValues } };

            // When
            var headers = new RequestHeaders(values);

            // Then
            headers.IfNoneMatch.ShouldBeSameAs(acceptValues);
        }

        [Fact]
        public void Should_return_empty_string_when_ifrange_headers_are_not_available()
        {
            // Given
            var values = new Dictionary<string, IEnumerable<string>>();

            // When
            var headers = new RequestHeaders(values);

            // Then
            headers.IfRange.ShouldBeEmpty();
        }

        [Fact]
        public void Should_return_ifrange_header_when_available()
        {
            // Given
            var acceptValues = new[] { "737060cd8c284d8af7ad3082f209582d" };
            var values = new Dictionary<string, IEnumerable<string>> { { "If-Range", acceptValues } };

            // When
            var headers = new RequestHeaders(values);

            // Then
            headers.IfRange.ShouldEqual(acceptValues[0]);
        }

        [Theory]
        [InlineData("if-range")]
        [InlineData("IF-RANGe")]
        public void Should_ignore_case_of_ifrange_header_name_when_retrieving_values(string headerName)
        {
            // Given
            var acceptValues = new[] { "737060cd8c284d8af7ad3082f209582d" };
            var values = new Dictionary<string, IEnumerable<string>> { { headerName, acceptValues } };

            // When
            var headers = new RequestHeaders(values);

            // Then
            headers.IfRange.ShouldEqual(acceptValues[0]);
        }

        [Fact]
        public void Should_return_min_date_when_ifunmodifiedsince_headers_are_not_available()
        {
            // Given
            var values = new Dictionary<string, IEnumerable<string>>();

            // When
            var headers = new RequestHeaders(values);

            // Then
            headers.IfUnmodifiedSince.ShouldEqual(DateTime.MinValue);
        }

        [Fact]
        public void Should_return_ifunmodifiedsince_when_available()
        {
            // Given
            var expectedDate = new DateTime(2011, 11, 15, 8, 12, 31);
            var acceptValues = new[] { "Tue, 15 Nov 2011 08:12:31 GMT" };
            var values = new Dictionary<string, IEnumerable<string>> { { "If-Unmodified-Since", acceptValues } };

            // When
            var headers = new RequestHeaders(values);

            // Then
            headers.IfUnmodifiedSince.ShouldEqual(expectedDate);
        }

        [Theory]
        [InlineData("if-unmodified-since")]
        [InlineData("If-UnmoDified-SInce")]
        public void Should_ignore_case_of_ifunmodifiedsince_header_name_when_retrieving_values(string headerName)
        {
            // Given
            var expectedDate = new DateTime(2011, 11, 15, 8, 12, 31);
            var acceptValues = new[] { "Tue, 15 Nov 2011 08:12:31 GMT" };
            var values = new Dictionary<string, IEnumerable<string>> { { headerName, acceptValues } };

            // When
            var headers = new RequestHeaders(values);

            // Then
            headers.IfUnmodifiedSince.ShouldEqual(expectedDate);
        }

        [Fact]
        public void Should_return_zero_when_maxforwards_headers_are_not_available()
        {
            // Given
            var values = new Dictionary<string, IEnumerable<string>>();

            // When
            var headers = new RequestHeaders(values);

            // Then
            headers.MaxForwards.ShouldEqual(0);
        }

        [Fact]
        public void Should_return_maxforwards_headers_when_available()
        {
            // Given
            var acceptValues = new[] { "12" };
            var values = new Dictionary<string, IEnumerable<string>> { { "Max-Forwards", acceptValues } };

            // When
            var headers = new RequestHeaders(values);

            // Then
            headers.MaxForwards.ShouldEqual(12);
        }

        [Theory]
        [InlineData("max-forwards")]
        [InlineData("MAX-Forwards")]
        public void Should_ignore_case_of_maxforwards_header_name_when_retrieving_values(string headerName)
        {
            // Given
            var acceptValues = new[] { "12" };
            var values = new Dictionary<string, IEnumerable<string>> { { headerName, acceptValues } };

            // When
            var headers = new RequestHeaders(values);

            // Then
            headers.MaxForwards.ShouldEqual(12);
        }

        [Fact]
        public void Should_return_empty_string_when_referer_headers_are_not_available()
        {
            // Given
            var values = new Dictionary<string, IEnumerable<string>>();

            // When
            var headers = new RequestHeaders(values);

            // Then
            headers.Referrer.ShouldBeEmpty();
        }

        [Fact]
        public void Should_return_referer_headers_when_available()
        {
            // Given
            var acceptValues = new[] { "http://nancyfx.org" };
            var values = new Dictionary<string, IEnumerable<string>> { { "Referer", acceptValues } };

            // When
            var headers = new RequestHeaders(values);

            // Then
            headers.Referrer.ShouldBeSameAs(acceptValues[0]);
        }

        [Theory]
        [InlineData("referer")]
        [InlineData("RefERer")]
        public void Should_ignore_case_of_referer_header_name_when_retrieving_values(string headerName)
        {
            // Given
            var acceptValues = new[] { "http://nancyfx.org" };
            var values = new Dictionary<string, IEnumerable<string>> { { headerName, acceptValues } };

            // When
            var headers = new RequestHeaders(values);

            // Then
            headers.Referrer.ShouldBeSameAs(acceptValues[0]);
        }

        [Fact]
        public void Should_return_empty_string_when_useragent_headers_are_not_available()
        {
            // Given
            var values = new Dictionary<string, IEnumerable<string>>();

            // When
            var headers = new RequestHeaders(values);

            // Then
            headers.UserAgent.ShouldBeEmpty();
        }

        [Fact]
        public void Should_return_useragent_headers_when_available()
        {
            // Given
            var acceptValues = new[] { "Mozilla/5.0 (Windows NT 5.1) AppleWebKit/535.1 (KHTML, like Gecko) Chrome/14.0.815.0 Safari/535.1" };
            var values = new Dictionary<string, IEnumerable<string>> { { "User-Agent", acceptValues } };

            // When
            var headers = new RequestHeaders(values);

            // Then
            headers.UserAgent.ShouldBeSameAs(acceptValues[0]);
        }

        [Theory]
        [InlineData("user-agent")]
        [InlineData("user-AGENT")]
        public void Should_ignore_case_of_useragent_header_name_when_retrieving_values(string headerName)
        {
            // Given
            var acceptValues = new[] { "Mozilla/5.0 (Windows NT 5.1) AppleWebKit/535.1 (KHTML, like Gecko) Chrome/14.0.815.0 Safari/535.1" };
            var values = new Dictionary<string, IEnumerable<string>> { { headerName, acceptValues } };

            // When
            var headers = new RequestHeaders(values);

            // Then
            headers.UserAgent.ShouldBeSameAs(acceptValues[0]);
        }
    }
}