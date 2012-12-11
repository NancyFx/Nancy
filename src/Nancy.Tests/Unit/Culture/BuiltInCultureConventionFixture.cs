using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using Nancy.IO;
using Xunit;

namespace Nancy.Tests.Unit.Culture
{
    public class BuiltInCultureConventionFixture
    {
        [Fact]
        public void Should_return_null_if_form_not_populated()
        {
            var context = new NancyContext();
            var request = new Request("GET", "/", "http");
            context.Request = request;

            var culture = Nancy.Conventions.BuiltInCultureConventions.FormCulture(context);

            Assert.Equal(null, culture);
        }

        [Fact]
        public void Should_return_culture_if_form_populated()
        {
            const string bodyContent = "CurrentCulture=en-GB";
            var memory = new MemoryStream();
            var writer = new StreamWriter(memory);
            writer.Write(bodyContent);
            writer.Flush();
            memory.Position = 0;

            var headers =
                new Dictionary<string, IEnumerable<string>>
                {
                    { "content-type", new[] { "application/x-www-form-urlencoded" } }
                };

            // When
            var request = new Request("POST", "/", headers, CreateRequestStream(memory), "http");
            request.Form.ToString();
            // Then
            ((string)request.Form.name).ShouldEqual("John Doe");


            //var context = new NancyContext();
            //var request = new Request("GET", "/", "http");
            //request.Form = new Dictionary<string, object>();
            //request.Form.Add("CurrentCulture", "en-GB");
            //context.Request = request;

            //var culture = Nancy.Conventions.BuiltInCultureConventions.FormCulture(context);

            Assert.Equal("en-GB", culture.Name);
        }

        private static RequestStream CreateRequestStream(Stream stream)
        {
            return RequestStream.FromStream(stream);
        }
    }
}
