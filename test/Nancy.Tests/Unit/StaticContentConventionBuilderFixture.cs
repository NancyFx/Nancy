namespace Nancy.Tests.Unit
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.IO;
    using System.Text;

    using Nancy.Conventions;
    using Nancy.Diagnostics;
    using Nancy.Responses;

    using Xunit;
    using Xunit.Extensions;

    public class StaticContentConventionBuilderFixture
    {
        private const string StylesheetContents = @"body {
	background-color: white;
}";

        private readonly string directory;

        public StaticContentConventionBuilderFixture()
        {
            this.directory = Environment.CurrentDirectory;

            GenericFileResponse.SafePaths.Add(this.directory);
        }

        [Fact]
        public void Should_retrieve_static_content_when_file_name_contains_url_encoded_spaces()
        {
            // Given
            // When
            var result = GetStaticContent("css", "space%20in%20name.css");

            // Then
            result.ShouldEqual(StylesheetContents);
        }

        [Fact]
        public void Should_retrieve_static_content_when_path_has_same_name_as_extension()
        {
            // Given
            // When
            var result = GetStaticContent("css", "styles.css");

            // Then
            result.ShouldEqual(StylesheetContents);
        }

        [Fact]
        public void Should_retrieve_static_content_when_virtual_directory_name_exists_in_static_route()
        {
            // Given
            // When
            var result = GetStaticContent("css", "strange-css-filename.css");

            // Then
            result.ShouldEqual(StylesheetContents);
        }

        [Fact]
        public void Should_retrieve_static_content_when_path_is_nested()
        {
            // Given
            // When
            var result = GetStaticContent("css/sub", "styles.css");

            // Then
            result.ShouldEqual(StylesheetContents);
        }

        [Fact]
        public void Should_retrieve_static_content_when_path_contains_nested_folders_with_duplicate_name()
        {
            // Given
            // When
            var result = GetStaticContent("css/css", "styles.css");

            // Then
            result.ShouldEqual(StylesheetContents);
        }

        [Fact]
        public void Should_retrieve_static_content_when_filename_contains_dot()
        {
            // Given
            // When
            var result = GetStaticContent("css", "dotted.filename.css");

            // Then
            result.ShouldEqual(StylesheetContents);
        }

        [Fact]
        public void Should_retrieve_static_content_when_path_contains_dot()
        {
            // Given
            // When
            var result = GetStaticContent("css/Sub.folder", "styles.css");

            // Then
            result.ShouldEqual(StylesheetContents);
        }

        [Fact]
        public void Should_skip_the_request_if_resource_is_outside_the_content_folder()
        {
            // Given
            // When
            var result = GetStaticContent("css", "../../outside/styles.css");

            // Then
            result.ShouldEqual("Static content returned an invalid response of (null)");
        }

        [Fact]
        public void Should_retrieve_static_content_when_root_is_relative_path()
        {
            // Given
            var resources = Path.Combine(directory, "Resources");
            var relativeRootFolder = Path.Combine(resources, @"../");

            // When
            var result = GetStaticContent("css", "styles.css", relativeRootFolder);

            // Then
            result.ShouldEqual(StylesheetContents);
        }

        [Fact]
        public void Should_throw_security_exception_when_content_path_points_to_root()
        {
            // Given
            var convention = StaticContentConventionBuilder.AddDirectory("/", "/");
            var request = new Request("GET", "/face.png", "http");
            var context = new NancyContext
            {
                Request = request
            };

            // When
            var exception = Record.Exception(() => convention.Invoke(context, directory));

            // Then
            exception.ShouldBeOfType<ArgumentException>();
        }

        [Fact]
        public void Should_throw_security_exception_when_content_path_is_null_and_requested_path_points_to_root()
        {
            // Given
            var convention = StaticContentConventionBuilder.AddDirectory("/");
            var request = new Request("GET", "/face.png", "http");
            var context = new NancyContext
            {
                Request = request
            };

            // When
            var exception = Record.Exception(() => convention.Invoke(context, directory));

            // Then
            exception.ShouldBeOfType<ArgumentException>();
        }

        [Fact]
        public void Should_return_not_modified_if_not_changed_and_conditional_request_on_etag_sent()
        {
            var initialResult = this.GetStaticContentResponse("css/css", "styles.css");
            var etag = initialResult.Headers["ETag"];
            var headers = new Dictionary<string, IEnumerable<string>> { { "If-None-Match", new[] { etag } } };

            var result = this.GetStaticContentResponse("css/css", "styles.css", headers: headers);

            result.StatusCode.ShouldEqual(HttpStatusCode.NotModified);
        }

        [Fact]
        public void Should_return_not_modified_if_not_changed_and_conditional_request_on_modified_sent()
        {
            var initialResult = this.GetStaticContentResponse("css/css", "styles.css");
            var moddedTime = initialResult.Headers["Last-Modified"];
            var headers = new Dictionary<string, IEnumerable<string>> { { "If-Modified-Since", new[] { moddedTime } } };

            var result = this.GetStaticContentResponse("css/css", "styles.css", headers: headers);

            result.StatusCode.ShouldEqual(HttpStatusCode.NotModified);
        }

        [Fact]
        public void Should_return_full_response_if_changed_and_conditional_request_on_etag_sent()
        {
            var initialResult = this.GetStaticContentResponse("css/css", "styles.css");
            var etag = initialResult.Headers["ETag"];
            var headers = new Dictionary<string, IEnumerable<string>> { { "If-None-Match", new[] { etag.Substring(1) } } };

            var result = this.GetStaticContentResponse("css/css", "styles.css", headers: headers);

            result.StatusCode.ShouldEqual(HttpStatusCode.OK);
        }

        [Fact]
        public void Should_return_full_response_if_changed_and_conditional_request_on_modified_sent()
        {
            var initialResult = this.GetStaticContentResponse("css/css", "styles.css");
            var moddedTimeString = initialResult.Headers["Last-Modified"];
            var moddedTime = DateTime.ParseExact(moddedTimeString, "R", CultureInfo.InvariantCulture, DateTimeStyles.None)
                                     .AddHours(-1);
            moddedTimeString = moddedTime.ToString("R", CultureInfo.InvariantCulture);
            var headers = new Dictionary<string, IEnumerable<string>> { { "If-Modified-Since", new[] { moddedTimeString } } };

            var result = this.GetStaticContentResponse("css/css", "styles.css", headers: headers);

            result.StatusCode.ShouldEqual(HttpStatusCode.OK);
        }

        [Fact]
        public void Not_modified_response_should_have_no_body()
        {
            var initialResult = this.GetStaticContentResponse("css/css", "styles.css");
            var etag = initialResult.Headers["ETag"];
            var headers = new Dictionary<string, IEnumerable<string>> { { "If-None-Match", new[] { etag } } };

            var result = this.GetStaticContent("css/css", "styles.css", headers: headers);

            result.ShouldEqual(string.Empty);
        }

        [Theory]
        [InlineData('"')]
        [InlineData('<')]
        [InlineData('>')]
        [InlineData('|')]
        public void Should_not_throw_exception_when_path_contains_invalid_filename_character(char invalidCharacter)
        {
            // Given
            var fileName = string.Format("name{0}.ext", invalidCharacter);

            // When
            var exception = Record.Exception(() => {
                this.GetStaticContent("css/css", fileName);
            });

            // Then
            exception.ShouldBeNull();
        }

        private string GetStaticContent(string virtualDirectory, string requestedFilename, string root = null, IDictionary<string, IEnumerable<string>> headers = null)
        {
            var response = this.GetStaticContentResponse(virtualDirectory, requestedFilename, root, headers);

            var fileResponse = response as GenericFileResponse;

            if (fileResponse != null)
            {
                using (var stream = new MemoryStream())
                {
                    fileResponse.Contents(stream);
                    return Encoding.UTF8.GetString(stream.GetBuffer(), 0, (int)stream.Length);
                }
            }

            return string.Format("Static content returned an invalid response of {0}", response == null ? "(null)" : response.GetType().ToString());
        }

        private Response GetStaticContentResponse(string virtualDirectory, string requestedFilename, string root = null, IDictionary<string, IEnumerable<string>> headers = null)
        {
            var context = GetContext(virtualDirectory, requestedFilename, headers);

            var resolver = GetResolver(virtualDirectory);

            var rootFolder = root ?? this.directory;

            GenericFileResponse.SafePaths.Add(rootFolder);

            var response = resolver.Invoke(context, rootFolder);
            return response;
        }

        private static NancyContext GetContext(string virtualDirectory, string requestedFilename, IDictionary<string, IEnumerable<string>> headers = null)
        {
            var resource = string.Format("/{0}/{1}", virtualDirectory, requestedFilename);

            var request = new Request(
                "GET",
                new Url { Path = resource, Scheme = "http" },
                headers: headers ?? new Dictionary<string, IEnumerable<string>>());

            var context = new NancyContext
            {
                Request = request,
                Trace = new DefaultRequestTrace
                {
                    TraceLog = new DefaultTraceLog()
                }
            };

            return context;
        }

        private static Func<NancyContext, string, Response> GetResolver(string virtualDirectory)
        {
            return StaticContentConventionBuilder.AddDirectory(virtualDirectory, "Resources/Assets/Styles");
        }
    }
}