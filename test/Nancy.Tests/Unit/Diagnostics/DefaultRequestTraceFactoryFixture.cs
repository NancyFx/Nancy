namespace Nancy.Tests.Unit.Diagnostics
{
    using System.Collections.Generic;
    using System.IO;
    using Nancy.Configuration;
    using Nancy.Diagnostics;
    using Nancy.IO;
    using Nancy.Testing;

    using Xunit;

    public class DefaultRequestTraceFactoryFixture
    {
        private DefaultRequestTraceFactory factory;
        private readonly Request request;

        public DefaultRequestTraceFactoryFixture()
        {
            this.factory = CreateFactory();
            this.request = CreateRequest();
        }

        [Fact]
        public void Should_create_default_trace_log_instance_when_error_tracing_is_activated()
        {
            // Given
            this.factory = CreateFactory(true);

            // When
            var trace = this.factory.Create(this.request);

            // Then
            trace.TraceLog.ShouldNotBeNull();
            trace.TraceLog.ShouldBeOfType<DefaultTraceLog>();
        }

        [Fact]
        public void Should_create_null_trace_log_instance_when_error_tracing_is_deactivated()
        {
            // Given
            this.factory = CreateFactory(false);

            // When
            var trace = this.factory.Create(this.request);

            // Then
            trace.TraceLog.ShouldNotBeNull();
            trace.TraceLog.ShouldBeOfType<NullLog>();
        }

        [Fact]
        public void Should_initialize_case_insensitive_items_collection_when_case_sensitivity_is_disabled()
        {
            using (new StaticConfigurationContext(x => x.CaseSensitive = false))
            {
                // Given
                var trace = this.factory.Create(this.request);

                // When
                trace.Items.Add("foo", "bar");

                // Then
                Record.Exception(() => trace.Items["FOO"]).ShouldBeNull();
            }
        }

        [Fact]
        public void Should_initialize_case_sensitive_items_collection_when_case_sensitivity_is_enabled()
        {
            using (new StaticConfigurationContext(x => x.CaseSensitive = true))
            {
                // Given
                var trace = this.factory.Create(this.request);

                // When
                trace.Items.Add("foo", "bar");

                // Then
                Assert.Throws<KeyNotFoundException>(() =>
                {
                    var value = trace.Items["FOO"];
                });
            }
        }

        [Fact]
        public void Should_create_request_info_from_request()
        {
            // Given
            // When
            var trace = this.factory.Create(this.request);

            // Then
            trace.RequestData.ContentType.Matches(this.request.Headers.ContentType).ShouldBeTrue();
            trace.RequestData.Headers.ShouldBeSameAs(this.request.Headers);
            trace.RequestData.Method.ShouldEqual(this.request.Method);
            trace.RequestData.Url.ShouldBeSameAs(this.request.Url);
        }

        [Fact]
        public void Should_not_create_response_info()
        {
            // Given
            // When
            var trace = this.factory.Create(this.request);

            // Then
            trace.ResponseData.ShouldBeNull();
        }

        private static DefaultRequestTraceFactory CreateFactory(bool displayErrorTraces = true)
        {
            var environment =
                new DefaultNancyEnvironment();

            environment.Tracing(
                enabled: true,
                displayErrorTraces: displayErrorTraces);

            return new DefaultRequestTraceFactory(environment);
        }

        private static Request CreateRequest()
        {
            return new Request(
                "GET",
                new Url(),
                RequestStream.FromStream(new MemoryStream()),
                new Dictionary<string, IEnumerable<string>>
                {
                    {"Content-Type", new[] {"text/plain"}}
                });
        }
    }
}