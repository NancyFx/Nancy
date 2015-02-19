namespace Nancy.Tests.Unit.Helpers
{
    using Nancy.Helpers;

    using Xunit;
    using Xunit.Extensions;

    public class HttpUtilityFixture
    {
        [Fact]
        public void ParseQueryString_respects_case_insensitive_setting()
        {
            // Given
            StaticConfiguration.CaseSensitive = false;
            var query = "key=value";

            // When
            var collection = HttpUtility.ParseQueryString(query);

            // Then
            collection["key"].ShouldEqual("value");
            collection["KEY"].ShouldEqual("value");
        }

        [Fact]
        public void ParseQueryString_respects_case_sensitive_setting()
        {
            // Given
            StaticConfiguration.CaseSensitive = true;
			var query = "key=value";

            // When
            var collection = HttpUtility.ParseQueryString(query);

            // Then
            collection["key"].ShouldEqual("value");
            collection["KEY"].ShouldBeNull();
        }

        [Fact]
        public void ParseQueryString_handles_duplicate_keys_when_case_insensitive()
        {
            // Given
            StaticConfiguration.CaseSensitive = false;
            var query = "key=value&key=value&KEY=VALUE";

            // When
            var collection = HttpUtility.ParseQueryString(query);

            // Then
            collection["key"].ShouldEqual("value,value,VALUE");
            collection["KEY"].ShouldEqual("value,value,VALUE");
        }

        [Fact]
        public void ParseQueryString_handles_duplicate_keys_when_case_sensitive()
        {
            // Given
            StaticConfiguration.CaseSensitive = true;
            var query = "key=value&key=value&KEY=VALUE";

            // When
            var collection = HttpUtility.ParseQueryString(query);

            // Then
            collection["key"].ShouldEqual("value,value");
            collection["KEY"].ShouldEqual("VALUE");
        }

        [Fact]
        public void ParseQueryString_explicit_case_insensitivity_overrides_global_setting()
        {
            // Given
            StaticConfiguration.CaseSensitive = true;
            var query = "key=value";

            // When
            var collection = HttpUtility.ParseQueryString(query, caseSensitive: false);

            // Then
            collection["key"].ShouldEqual("value");
            collection["KEY"].ShouldEqual("value");
        }

        [Fact]
        public void ParseQueryString_explicit_case_sensitivity_overrides_global_setting()
        {
            // Given
            StaticConfiguration.CaseSensitive = false;
            var query = "key=value";

            // When
            var collection = HttpUtility.ParseQueryString(query, caseSensitive: true);

            // Then
            collection["key"].ShouldEqual("value");
            collection["KEY"].ShouldBeNull();
        }

        [Fact]
        public void ParseQueryString_handles_keys_without_values()
        {
            // Given
            var query = "key1&key2";

            // When
            var collection = HttpUtility.ParseQueryString(query);

            // Then
            collection["key1"].ShouldEqual("key1");
            collection["key2"].ShouldEqual("key2");
        }

        [Fact]
        public void ParseQueryString_handles_duplicate_keys_when_one_has_no_value()
        {
            // Given
            var query = "key&key=value";

            // When
            var collection = HttpUtility.ParseQueryString(query);

            // Then
            collection["key"].ShouldEqual("key,value");
        }

        [Fact]
        public void ParseQueryString_handles_duplicate_keys_when_they_have_no_values()
        {
            // Given
            var query = "key&key";

            // When
            var collection = HttpUtility.ParseQueryString(query);

            // Then
            collection["key"].ShouldEqual("key,key");
        }

		[Theory]
		[InlineData("/a/a&/b&/c")]
		[InlineData("/build/app-transitions-css/app-transitions-css-min.css&/build/widget-base/assets/skins/sam/widget-base.css&/build/scrollview-base/assets/skins/sam/scrollview-base.css&/build/scrollview-scrollbars/assets/skins/sam/scrollview-scrollbars.css&/build/widget-stack/assets/skins/sam/widget-stack.css&/build/overlay/assets/skins/sam/overlay.css&/build/console/assets/skins/sam/console.css")]
		public void ParseQueryString_handles_irregular_yui_format(string query)
		{
			Assert.DoesNotThrow(() =>
			{
				var collection = HttpUtility.ParseQueryString(query);
	
				collection.ShouldNotBeNull();
			});
		}
    }
}