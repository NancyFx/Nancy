namespace Nancy.Tests.Unit
{
    using System;
    using System.IO;
    using System.Text;
    using Nancy.Conventions;
    using Nancy.Responses;
    using Xunit;

    public class StaticContentConventionBuilderFixture
    {
        private const string StylesheetContents = @"body {
	background-color: white;
}";
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
            result.ShouldBeNull();
        }

        [Fact]
        public void Should_retrieve_static_content_when_root_is_relative_path()
        {
            // Given
            var resources = Path.Combine(Environment.CurrentDirectory, "Resources");
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
            var exception = Record.Exception(() => convention.Invoke(context, Environment.CurrentDirectory));

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
            var exception = Record.Exception(() => convention.Invoke(context, Environment.CurrentDirectory));

            // Then
            exception.ShouldBeOfType<ArgumentException>();
        }

        private static string GetStaticContent(string virtualDirectory, string requestedFilename, string root = null)
        {
            var resource =
                string.Format("/{0}/{1}", virtualDirectory, requestedFilename);

            var context =
                new NancyContext
                {
                    Request = new Request("GET", resource, "http")
                };

            var resolver =
                StaticContentConventionBuilder.AddDirectory(virtualDirectory, "Resources/Assets/Styles");

            var rootFolder = root ?? Environment.CurrentDirectory;

            GenericFileResponse.SafePaths.Add(rootFolder);

            var response =
                resolver.Invoke(context, rootFolder) as GenericFileResponse;

            if (response != null)
            {
                using (var stream = new MemoryStream())
                {
                    response.Contents(stream);
                    return Encoding.UTF8.GetString(stream.GetBuffer(), 0, (int)stream.Length);
                }
            }

            return null;
        }
    }
}