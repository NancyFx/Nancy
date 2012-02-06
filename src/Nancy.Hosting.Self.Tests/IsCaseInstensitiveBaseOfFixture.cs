using System;
using Xunit;
using Nancy.Tests;

namespace Nancy.Hosting.Self.Tests
{
	public class IsCaseInstensitiveBaseOfFixture
	{
		private readonly Uri baseUri = new Uri("http://host/path/path/file");
		private readonly Uri baseSlashUri = new Uri("http://host/path/path/");

		[Fact]
		public void url_should_be_base_of_sub_directory()
		{
			var isBaseOf = baseUri.IsCaseInsensitiveBaseOf(new Uri("http://host/path/path/file/"));

			isBaseOf.ShouldBeTrue();
		}

		[Fact]
		public void url_should_be_base_of_sub_path_with_fragment()
		{
			var isBaseOf = baseUri.IsCaseInsensitiveBaseOf(new Uri("http://host/path/path/file#fragment"));

			isBaseOf.ShouldBeTrue();
		}

		[Fact]
		public void url_should_be_base_of_path_with_more_dirs()
		{
			var isBaseOf = baseUri.IsCaseInsensitiveBaseOf(new Uri("http://host/path/path/file/MoreDir/"));

			isBaseOf.ShouldBeTrue();
		}

		[Fact]
		public void url_should_be_base_of_sub_path_with_file_and_query()
		{
			var isBaseOf = baseUri.IsCaseInsensitiveBaseOf(new Uri("http://host/path/path/file/OtherFile?Query"));

			isBaseOf.ShouldBeTrue();
		}

		[Fact]
		public void url_should_be_base_of_path_with_extra_slash()
		{
			var isBaseOf = baseUri.IsCaseInsensitiveBaseOf(new Uri("http://host/path/path/file/"));

			isBaseOf.ShouldBeTrue();
		}

		[Fact]
		public void url_should_be_base_of_sub_file()
		{
			var isBaseOf = baseUri.IsCaseInsensitiveBaseOf(new Uri("http://host/path/path/file"));

			isBaseOf.ShouldBeTrue();
		}

		[Fact]
		public void url_should_not_be_base_of_other_scheme()
		{
			var isBaseOf = baseUri.IsCaseInsensitiveBaseOf(new Uri("https://host/path/path/file"));

			isBaseOf.ShouldBeFalse();
		}

		[Fact]
		public void url_should_not_be_base_of_other_host()
		{
			var isBaseOf = baseUri.IsCaseInsensitiveBaseOf(new Uri("http://otherhost/path/path/file"));

			isBaseOf.ShouldBeFalse();
		}


		[Fact]
		public void url_should_not_be_base_of_other_port()
		{
			var isBaseOf = baseUri.IsCaseInsensitiveBaseOf(new Uri("http://otherhost:8080/path/path/file"));

			isBaseOf.ShouldBeFalse();
		}

		[Fact]
		public void url_should_be_base_of_host_with_different_casing()
		{
			var isBaseOf = baseUri.IsCaseInsensitiveBaseOf(new Uri("http://Host/path/path/file"));

			isBaseOf.ShouldBeTrue();
		}

		[Fact]
		public void url_should_be_base_of_exact_path_without_trailing_slash()
		{
			var isBaseOf = baseSlashUri.IsCaseInsensitiveBaseOf(new Uri("http://host/path/path"));

			isBaseOf.ShouldBeTrue();
		}

		[Fact]
		public void url_should_be_base_of_exact_path_without_trailing_slash_with_query()
		{
			var isBaseOf = baseSlashUri.IsCaseInsensitiveBaseOf(new Uri("http://host/path/path?query"));

			isBaseOf.ShouldBeTrue();
		}

		[Fact]
		public void url_should_be_base_of_exact_path_without_trailing_slash_with_fragment()
		{
			var isBaseOf = baseSlashUri.IsCaseInsensitiveBaseOf(new Uri("http://host/path/path#Fragment"));

			isBaseOf.ShouldBeTrue();
		}

		[Fact]
		public void url_should_not_be_base_of_other_path()
		{
			var isBaseOf = baseSlashUri.IsCaseInsensitiveBaseOf(new Uri("http://host/path/path2/"));

			isBaseOf.ShouldBeFalse();
		}

		[Fact]
		public void url_should_be_base_of_same_path_with_different_host_casing()
		{
			var isBaseOf = baseSlashUri.IsCaseInsensitiveBaseOf(new Uri("http://Host/path/path/"));

			isBaseOf.ShouldBeTrue();
		}


		[Fact]
		public void url_should_be_base_of_same_path_with_different_path_casing()
		{
			var isBaseOf = baseSlashUri.IsCaseInsensitiveBaseOf(new Uri("http://host/Path/PATH/"));

			isBaseOf.ShouldBeTrue();
		}
	}
}