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

		private static string GetStaticContent(string virtualDirectory, string requestedFilename)
		{
			var resource = 
                string.Format("{0}/{1}", virtualDirectory, requestedFilename);

			var context = 
                new NancyContext { Request = new Request("GET", resource, "http") };

			var resolver = 
                StaticContentConventionBuilder.AddDirectory("css", @"Resources\Assets\Styles");

			GenericFileResponse.SafePaths.Add(Environment.CurrentDirectory);

			var response = 
                resolver.Invoke(context, Environment.CurrentDirectory) as GenericFileResponse;

			using (var stream = new MemoryStream())
			{
				response.Contents(stream);
				return Encoding.UTF8.GetString(stream.GetBuffer(), 0, (int)stream.Length);
			}
		}
	}
}