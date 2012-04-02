using System;
using System.IO;
using System.Text;
using FakeItEasy;
using Nancy.Conventions;
using Nancy.Responses;
using Xunit;

namespace Nancy.Tests.Unit
{
	public class StaticConventBuilderFixture
	{
		private const string StylesheetContents = @"body {
	background-color: white;
}";

		[Fact]
		public void Static_routes_can_have_same_name_as_extension()
		{
			getStaticContent("css", "styles.css");
		}

		[Fact]
		public void Virtual_directory_name_can_exist_in_static_route()
		{
			getStaticContent("css", "strange-css-filename.css");
		}

		[Fact]
		public void Static_content_can_be_nested()
		{
			getStaticContent("css/sub", "styles.css");
		}

		[Fact]
		public void Static_content_can_be_nested_with_duplicate_name()
		{
			getStaticContent("css/css", "styles.css");
		}

        [Fact]
        public void Path_with_dot_in_it_doesnt_cause_problems()
        {
            getStaticContent("css", "dotted.filename.css");
            getStaticContent("css/Sub.folder", "styles.css");
        }

		private void getStaticContent(string virtualDirectory, string requestedFilename)
		{
			var resource = string.Format("{0}/{1}", virtualDirectory, requestedFilename);
			var nancyCtx = new NancyContext() { Request = new Request("GET", resource, "http") };

			var resolver = StaticContentConventionBuilder.AddDirectory("css", @"Resources\Assets\Styles");

			GenericFileResponse.SafePaths.Add(Environment.CurrentDirectory);
			var response = resolver.Invoke(nancyCtx, Environment.CurrentDirectory) as GenericFileResponse;

			Assert.NotNull(response);
			Assert.True(requestedFilename.Equals(response.Filename, StringComparison.CurrentCultureIgnoreCase));

			using (var ms = new MemoryStream())
			{
				response.Contents(ms);
				var css = Encoding.UTF8.GetString(ms.GetBuffer(), 0, (int)ms.Length);
				Assert.Equal(StylesheetContents, css);
			}
		}
	}
}