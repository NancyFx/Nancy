using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Nancy.Hosting.Aspnet.Tests
{
    using System;
    using System.Collections.Specialized;
    using System.IO;
    using System.Web;
    using Nancy.Cookies;
    using FakeItEasy;
    using Nancy.Hosting.Aspnet;
    using Xunit;

    public class NancyHandlerFixture
    {
        private readonly NancyHandler handler;
        private readonly HttpContextBase context;
        private readonly HttpRequestBase request;
        private readonly HttpResponseBase response;
        private readonly INancyEngine engine;
        private readonly NameValueCollection formData;
        HttpFileCollectionBase fileCollection;
        IList<HttpPostedFileBase> files;

        public NancyHandlerFixture()
        {
            this.context = A.Fake<HttpContextBase>();
            this.request = A.Fake<HttpRequestBase>();
            this.response = A.Fake<HttpResponseBase>();
            this.engine = A.Fake<INancyEngine>();
            this.handler = new NancyHandler(engine);
            this.formData = new NameValueCollection();
            this.fileCollection = A.Fake<HttpFileCollectionBase>();

            A.CallTo(() => this.request.Form).ReturnsLazily(() => this.formData);
            A.CallTo(() => this.request.Url).Returns(new Uri("http://www.foo.com"));
            A.CallTo(() => this.request.InputStream).Returns(new MemoryStream());
            A.CallTo(() => this.request.Headers).Returns(new NameValueCollection());
            A.CallTo(() => this.request.AppRelativeCurrentExecutionFilePath).Returns("~/foo");

            A.CallTo(() => this.context.Request).Returns(this.request);
            A.CallTo(() => this.context.Response).Returns(this.response);

            A.CallTo(() => this.response.OutputStream).Returns(new MemoryStream());
        }

        [Fact]
        public void Should_invoke_engine_with_request_set_to_form_method_value_when_available()
        {
            // Given
            this.formData.Add("_method", "DELETE");
            A.CallTo(() => this.request.HttpMethod).Returns("POST");

            // When
            this.handler.ProcessRequest(this.context);

            // Then
            A.CallTo(() => this.engine.HandleRequest(A<Request>.That.Matches(x => x.Method.Equals("DELETE")))).MustHaveHappened();
        }

        [Fact]
        public void Should_invoke_engine_with_requested_method()
        {
            // Given
            A.CallTo(() => this.request.HttpMethod).Returns("POST");

            // When
            this.handler.ProcessRequest(this.context);

            // Then
            A.CallTo(() => this.engine.HandleRequest(A<Request>.That.Matches(x => x.Method.Equals("POST")))).MustHaveHappened();
        }

        [Fact]
        public void Should_output_the_responses_cookies()
        {
            var cookie1 = A.Fake<INancyCookie>();
            var cookie2 = A.Fake<INancyCookie>();
            var r = new Response();
            r.AddCookie(cookie1).AddCookie(cookie2);

            A.CallTo(() => cookie1.ToString()).Returns("the first cookie");
            A.CallTo(() => cookie2.ToString()).Returns("the second cookie");
            
            SetupRequestProcess(r);
            
            this.handler.ProcessRequest(context);

            A.CallTo(() => this.response.AddHeader("Set-Cookie", "the first cookie")).MustHaveHappened();
            A.CallTo(() => this.response.AddHeader("Set-Cookie", "the second cookie")).MustHaveHappened();
        }

        [Fact]
        public void Should_pass_the_aspnet_request_file_collection_to_the_request_as_nancy_files()
        {
            // Given
            SetupMultipartRequest();
            Request requestResult = null;
            A.CallTo(() => this.engine.HandleRequest(A<Request>.Ignored)).Invokes(x =>
            {
                requestResult = x.Arguments.Get<Request>(0);
            }).Returns(new Response());

            // When
            this.handler.ProcessRequest(this.context);

            // Then
            Assert.Equal(requestResult.Files.Count(), 3);
            Assert.Equal(requestResult.Files.Select(x => x.FileName).ToArray(), this.files.Select(x => x.FileName).ToArray());
            Assert.Equal(requestResult.Files.Select(x => x.FileName).ToArray(), this.files.Select(x => x.FileName).ToArray());
        }

        [Fact]
        public void Should_pass_the_aspnet_request_form_values_to_the_nancy_request()
        {
            // Given
            SetupMultipartRequest();
            Request requestResult = null;
            A.CallTo(() => this.engine.HandleRequest(A<Request>.Ignored)).Invokes(x =>
            {
                requestResult = x.Arguments.Get<Request>(0);
            }).Returns(new Response());

            // When
            this.handler.ProcessRequest(this.context);

            Assert.Equal((string)requestResult.Form["Name"], "Chris");
            Assert.Equal((string)requestResult.Form.Name, "Chris");
        }

        private void SetupRequestProcess(Response response)
        {
            A.CallTo(() => this.request.AppRelativeCurrentExecutionFilePath).Returns("~/about");
            A.CallTo(() => this.request.Url).Returns(new Uri("http://ihatedummydata.com/about"));
            A.CallTo(() => this.request.HttpMethod).Returns("GET");
            A.CallTo(() => this.engine.HandleRequest(A<Request>.Ignored.Argument)).Returns(response);
        }

        private void SetupMultipartRequest()
        {
            A.CallTo(() => this.request.HttpMethod).Returns("POST");
            this.request.ContentType = "multipart/form-data";

            A.CallTo(() => this.request.Files).Returns(fileCollection);
            var formData = new NameValueCollection {{"Name", "Chris"}};
            A.CallTo(() => this.request.Form).Returns(formData);

            this.files = A.CollectionOfFake<HttpPostedFileBase>(3);
            for (int i = 0; i < this.files.Count; i++)
            {
                var content = new MemoryStream(Encoding.UTF8.GetBytes("Some test context text"));
                A.CallTo(() => this.files[0].FileName).Returns("TestFile" + i);
                A.CallTo(() => this.files[0].ContentType).Returns("text/html");
                A.CallTo(() => this.files[0].ContentLength).Returns((int)content.Length);
                A.CallTo(() => this.files[0].InputStream).Returns(content);
            }

            A.CallTo(() => this.fileCollection.GetEnumerator()).Returns(this.files.GetEnumerator());
        }
    }
}