namespace Nancy.Session.InProc.Tests.InProcSessionsManagement.ByQueryStringParam
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Nancy.Session.InProc.InProcSessionsManagement;
    using Nancy.Session.InProc.InProcSessionsManagement.ByQueryStringParam;
    using Xunit;

    public class ResponseManipulatorForSessionFixture
    {
        private readonly NancyContext context;
        private readonly string parameterName;
        private readonly ResponseManipulatorForSession responseManipulatorForSession;
        private readonly SessionIdentificationData sessionIdentificationData;

        public ResponseManipulatorForSessionFixture()
        {
            this.responseManipulatorForSession = new ResponseManipulatorForSession();

            this.context = new NancyContext
            {
                Response = new Response(),
                Request = new Request("GET", "http://www.google.be")
            };
            this.sessionIdentificationData = new SessionIdentificationData
            {
                SessionId = "01SessionId",
                Hmac = new byte[] {211, 81, 204, 0, 47, 124}
            };
            this.parameterName = "SID";
        }

        [Fact]
        public void Given_null_context_then_throws()
        {
            Assert.Throws<ArgumentNullException>(
                () =>
                    this.responseManipulatorForSession.ModifyResponseToRedirectToSessionAwareUrl(
                        null,
                        this.sessionIdentificationData,
                        this.parameterName));
        }

        [Fact]
        public void Given_null_session_identification_data_then_throws()
        {
            Assert.Throws<ArgumentNullException>(
                () =>
                    this.responseManipulatorForSession.ModifyResponseToRedirectToSessionAwareUrl(
                        this.context,
                        null,
                        this.parameterName));
        }

        [Fact]
        public void Given_null_parameter_name_then_throws()
        {
            Assert.Throws<ArgumentNullException>(
                () =>
                    this.responseManipulatorForSession.ModifyResponseToRedirectToSessionAwareUrl(
                        this.context,
                        this.sessionIdentificationData,
                        null));
        }

        [Fact]
        public void Given_empty_parameter_name_then_throws()
        {
            Assert.Throws<ArgumentNullException>(
                () =>
                    this.responseManipulatorForSession.ModifyResponseToRedirectToSessionAwareUrl(
                        this.context,
                        this.sessionIdentificationData,
                        string.Empty));
        }

        [Fact]
        public void Given_whitespace_parameter_name_then_throws()
        {
            Assert.Throws<ArgumentNullException>(
                () =>
                    this.responseManipulatorForSession.ModifyResponseToRedirectToSessionAwareUrl(
                        this.context,
                        this.sessionIdentificationData,
                        " "));
        }

        [Fact]
        public void Given_context_without_request_then_throws()
        {
            var contextWithoutRequest = new NancyContext
            {
                Response = new Response()
            };
            Assert.Throws<ArgumentException>(
                () =>
                    this.responseManipulatorForSession.ModifyResponseToRedirectToSessionAwareUrl(
                        contextWithoutRequest,
                        this.sessionIdentificationData,
                        this.parameterName));
        }

        [Fact]
        public void Given_context_without_response_then_throws()
        {
            var contextWithoutResponse = new NancyContext
            {
                Request = new Request("GET", "http://www.google.be")
            };
            Assert.Throws<ArgumentException>(
                () =>
                    this.responseManipulatorForSession.ModifyResponseToRedirectToSessionAwareUrl(
                        contextWithoutResponse,
                        this.sessionIdentificationData,
                        this.parameterName));
        }

        [Fact]
        public void Changes_http_status_code_of_reponse_to_302()
        {
            this.responseManipulatorForSession.ModifyResponseToRedirectToSessionAwareUrl(
                this.context,
                this.sessionIdentificationData,
                this.parameterName);
            Assert.Equal(this.context.Response.StatusCode, HttpStatusCode.Found);
        }

        [Fact]
        public void Given_request_without_query_then_creates_query_for_location_header()
        {
            var expectedLocationHeaderValue = this.context.Request.Url +
                                              "?" + this.parameterName +
                                              "=" + this.sessionIdentificationData;

            this.responseManipulatorForSession.ModifyResponseToRedirectToSessionAwareUrl(
                this.context,
                this.sessionIdentificationData,
                this.parameterName);

            var locationHeader = this.context.Response.Headers
                .FirstOrDefault(header => header.Key.Equals("Location", StringComparison.OrdinalIgnoreCase));
            Assert.NotNull(locationHeader);
            Assert.True(new Uri(expectedLocationHeaderValue).Equals(new Uri(locationHeader.Value)));
        }

        [Fact]
        public void Given_request_with_query_then_creates_query_for_location_header()
        {
            this.context.Request = new Request("GET", "http://www.google.be?value=test&process=3");
            var expectedLocationHeaderValue = this.context.Request.Url +
                                              "&" + this.parameterName +
                                              "=" + this.sessionIdentificationData;

            this.responseManipulatorForSession.ModifyResponseToRedirectToSessionAwareUrl(
                this.context,
                this.sessionIdentificationData,
                this.parameterName);

            var locationHeader = this.context.Response.Headers
                .FirstOrDefault(header => header.Key.Equals("Location", StringComparison.OrdinalIgnoreCase));
            Assert.NotNull(locationHeader);
            Assert.True(new Uri(expectedLocationHeaderValue).Equals(new Uri(locationHeader.Value)));
        }

        [Fact]
        public void Given_request_already_has_a_session_id_then_replaces_that_session_id()
        {
            this.context.Request = new Request("GET",
                "http://www.google.be:624?value=test&process=3&" + this.parameterName + "=ABC123&page=14");
            var expectedLocationHeaderValue = "http://www.google.be:624?value=test&process=3" +
                                              "&" + this.parameterName +
                                              "=" + this.sessionIdentificationData +
                                              "&page=14";

            this.responseManipulatorForSession.ModifyResponseToRedirectToSessionAwareUrl(
                this.context,
                this.sessionIdentificationData,
                this.parameterName);

            var locationHeader = this.context.Response.Headers
                .FirstOrDefault(header => header.Key.Equals("Location", StringComparison.OrdinalIgnoreCase));
            Assert.NotNull(locationHeader);
            Assert.True(new Uri(expectedLocationHeaderValue).Equals(new Uri(locationHeader.Value)));
        }

        [Fact]
        public void Given_request_already_has_a_session_id_with_encoded_characters_then_replaces_that_session_id()
        {
            this.context.Request = new Request("GET",
                "http://www.google.be:624?value=test&process=3&" + this.parameterName + "=ABC%C2%2F23&page=14");
            var expectedLocationHeaderValue = "http://www.google.be:624?value=test&process=3" +
                                              "&" + this.parameterName +
                                              "=" + this.sessionIdentificationData +
                                              "&page=14";

            this.responseManipulatorForSession.ModifyResponseToRedirectToSessionAwareUrl(
                this.context,
                this.sessionIdentificationData,
                this.parameterName);

            var locationHeader = this.context.Response.Headers
                .FirstOrDefault(header => header.Key.Equals("Location", StringComparison.OrdinalIgnoreCase));
            Assert.NotNull(locationHeader);
            Assert.True(new Uri(expectedLocationHeaderValue).Equals(new Uri(locationHeader.Value)));
        }

        [Fact]
        public void Given_session_identifier_is_lowercase_url_encoded()
        {
            this.sessionIdentificationData.SessionId = "/bOu§¨";
            const string encodedSessionId = "%2fbOu%c2%a7%c2%a8";
            const string expectedParameterValue = "01HMAC98" + encodedSessionId;
            var expectedLocationHeaderValue = this.context.Request.Url + "/" +
                                              "?" + this.parameterName +
                                              "=" + expectedParameterValue;

            this.responseManipulatorForSession.ModifyResponseToRedirectToSessionAwareUrl(
                this.context,
                this.sessionIdentificationData,
                this.parameterName);

            var locationHeader = this.context.Response.Headers
                .FirstOrDefault(header => header.Key.Equals("Location", StringComparison.OrdinalIgnoreCase));
            Assert.NotNull(locationHeader);
            Assert.True(new Uri(expectedLocationHeaderValue).Equals(new Uri(locationHeader.Value)));
        }

        [Fact]
        public void Given_response_already_has_a_location_header_then_replaces_header_value()
        {
            var headers = new Dictionary<string, IEnumerable<string>>();
            headers.Add("location", new[] {"http://www.github.com/nancyfx"});
            this.context.Request = new Request(this.context.Request.Method, this.context.Request.Url, null, headers);

            var expectedLocationHeaderValue = this.context.Request.Url +
                                              "?" + this.parameterName +
                                              "=" + this.sessionIdentificationData;

            this.responseManipulatorForSession.ModifyResponseToRedirectToSessionAwareUrl(
                this.context,
                this.sessionIdentificationData,
                this.parameterName);

            var locationHeader = this.context.Response.Headers
                .FirstOrDefault(header => header.Key.Equals("Location", StringComparison.OrdinalIgnoreCase));
            Assert.NotNull(locationHeader);
            Assert.True(new Uri(expectedLocationHeaderValue).Equals(new Uri(locationHeader.Value)));
        }
    }
}