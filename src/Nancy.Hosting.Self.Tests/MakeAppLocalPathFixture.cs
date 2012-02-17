using System;
using Nancy.Tests;
using Xunit;

namespace Nancy.Hosting.Self.Tests
{
	public class MakeAppLocalPathFixture
	{
		[Fact]
		public void path_should_represent_local_path()
		{
			var result = new Uri("http://host/base/").MakeAppLocalPath(new Uri("http://host/base/rel"));
			
			result.ShouldEqual("/rel");
		}


		[Fact]
		public void root_path_should_be_only_slash()
		{
			var result = new Uri("http://host/base/").MakeAppLocalPath(new Uri("http://host/base/"));

			result.ShouldEqual("/");
		}

		[Fact]
		public void slashless_root_path_should_be_only_slash()
		{
			var result = new Uri("http://host/base/").MakeAppLocalPath(new Uri("http://host/base"));

			result.ShouldEqual("/");
		}


		[Fact]
		public void path_casing_should_be_full_uri_one()
		{
			var result = new Uri("http://host/base/").MakeAppLocalPath(new Uri("http://host/base/ReL"));

			result.ShouldEqual("/ReL");
		}

		[Fact]
		public void site_root_is_supported()
		{
			var result = new Uri("http://host/").MakeAppLocalPath(new Uri("http://host/rel/file"));

			result.ShouldEqual("/rel/file");
		}

		[Fact]
		public void site_root_slashless_path_is_supported()
		{
			var result = new Uri("http://host/").MakeAppLocalPath(new Uri("http://host"));

			result.ShouldEqual("/");
		}
	}
}