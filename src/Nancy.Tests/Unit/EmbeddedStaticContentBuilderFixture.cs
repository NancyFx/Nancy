using System;
using System.IO;
using System.Text;
using Nancy.Conventions;
using Nancy.Responses;
using Xunit;

namespace Nancy.Tests.Unit
{
	public class EmbeddedStaticContentConventionBuilderFixture
	{
		[Fact]
		public void Virtual_directory_name_can_exist_in_static_route()
		{
			getStaticContent("image", "zip-image.png");

			getStaticContent("image", "zip.png");
		}

		[Fact]
		public void Static_content_can_be_nested()
		{
			getStaticContent("image/image/sub", "zip.png");
		}

		private void getStaticContent(string virtualDirectory, string requestedFilename)
		{
			const string rootNamespace = "Nancy.Tests.Resources.Embeded.Assets.Image";
			var resourceUri = string.Format("{0}/{1}", virtualDirectory, requestedFilename);
			var nancyCtx = new NancyContext { Request = new Request("GET", resourceUri, "http") };

			var resolver = EmbeddedStaticContentConventionBuilder.MapVirtualDirectory("Image", rootNamespace, GetType().Assembly);

			var response = resolver.Invoke(nancyCtx, "not-used-by-delegate") as StreamResponse;

			Assert.NotNull(response);

			using (var ms = new MemoryStream())
			{
				response.Contents(ms);

				var missingLetter = resourceUri.Remove(0, 1);
				resourceUri = missingLetter.Insert(0, "I");
				var resource = String.Concat("Nancy.Tests.Resources.Embeded.Assets.", resourceUri.Replace("/", "."));
				
				using (var res = GetType().Assembly.GetManifestResourceStream(resource))
				{
					Assert.NotNull(res);
					var bytes = new byte[res.Length];
					res.Read(bytes, 0, bytes.Length);

					Assert.Equal(bytes, ms.GetBuffer());
				}
			}
		}
	}
}