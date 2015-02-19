namespace Nancy.Tests.Unit.Diagnostics
{
    using System.Collections.Generic;
    using System.IO;

    using Nancy.Diagnostics;
    using Nancy.IO;
    using Nancy.Testing;

    using Xunit;

    public class DefaultRequestTraceFactoryFixture
    {
        private readonly DefaultRequestTraceFactory factory;
        private readonly Request request;

        public DefaultRequestTraceFactoryFixture()
        {
            this.factory = new DefaultRequestTraceFactory();
            this.request = CreateRequest();
        }

        [Fact]
        public void Should_create_default_trace_log_instance_when_error_tracing_is_activated()
        {
            using (new StaticConfigurationContext(x => x.DisableErrorTraces = false))
            {
                // Given
                // When
                var trace = this.factory.Create(this.request);

                // Then
                trace.TraceLog.ShouldNotBeNull();
                trace.TraceLog.ShouldBeOfType<DefaultTraceLog>();
            }
        }

        [Fact]
        public void Should_create_null_trace_log_instance_when_error_tracing_is_deactivated()
        {
            using (new StaticConfigurationContext(x => x.DisableErrorTraces = true))
            {
                // Given
                // When
                var trace = this.factory.Create(this.request);

                // Then
                trace.TraceLog.ShouldNotBeNull();
                trace.TraceLog.ShouldBeOfType<NullLog>();
            }
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
                Assert.DoesNotThrow(() =>
                {
                    var value = trace.Items["FOO"];
                });
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
            trace.RequestData.ContentType.ShouldEqual(request.Headers.ContentType);
            trace.RequestData.Headers.ShouldBeSameAs(request.Headers);
            trace.RequestData.Method.ShouldEqual(request.Method);
            trace.RequestData.Url.ShouldBeSameAs(request.Url);
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