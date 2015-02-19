namespace Nancy.Hosting.Self.Tests
{
    using System;

    using Nancy.Tests;

    using Xunit;

    public class IsCaseInstensitiveBaseOfFixture
    {
        private readonly Uri baseUri = new Uri("http://host/path/path/file");
        private readonly Uri baseSlashUri = new Uri("http://host/path/path/");
        private readonly Uri baseLocalHostUri = new Uri("http://localhost/path/path/");

        [Fact]
        public void url_should_be_base_of_sub_directory()
        {
            // Given, When
            var isBaseOf = baseUri.IsCaseInsensitiveBaseOf(new Uri("http://host/path/path/file/"));

            // Then
            isBaseOf.ShouldBeTrue();
        }

        [Fact]
        public void url_should_be_base_of_sub_path_with_fragment()
        {
            // Given, When
            var isBaseOf = baseUri.IsCaseInsensitiveBaseOf(new Uri("http://host/path/path/file#fragment"));

            // Then
            isBaseOf.ShouldBeTrue();
        }

        [Fact]
        public void url_should_be_base_of_path_with_more_dirs()
        {
            // Given, When
            var isBaseOf = baseUri.IsCaseInsensitiveBaseOf(new Uri("http://host/path/path/file/MoreDir/"));

            // Then
            isBaseOf.ShouldBeTrue();
        }

        [Fact]
        public void url_should_be_base_of_sub_path_with_file_and_query()
        {
            // Given, When
            var isBaseOf = baseUri.IsCaseInsensitiveBaseOf(new Uri("http://host/path/path/file/OtherFile?Query"));

            // Then
            isBaseOf.ShouldBeTrue();
        }

        [Fact]
        public void url_should_be_base_of_path_with_extra_slash()
        {
            // Given, When
            var isBaseOf = baseUri.IsCaseInsensitiveBaseOf(new Uri("http://host/path/path/file/"));

            // Then
            isBaseOf.ShouldBeTrue();
        }

        [Fact]
        public void url_should_be_base_of_sub_file()
        {
            // Given, When
            var isBaseOf = baseUri.IsCaseInsensitiveBaseOf(new Uri("http://host/path/path/file"));

            // Then
            isBaseOf.ShouldBeTrue();
        }

        [Fact]
        public void url_should_not_be_base_of_other_scheme()
        {
            // Given, When
            var isBaseOf = baseUri.IsCaseInsensitiveBaseOf(new Uri("https://host/path/path/file"));

            // Then
            isBaseOf.ShouldBeFalse();
        }

        [Fact]
        public void url_should_not_be_base_of_other_host()
        {
            // Given, When
            var isBaseOf = baseUri.IsCaseInsensitiveBaseOf(new Uri("http://otherhost/path/path/file"));

            // Then
            isBaseOf.ShouldBeFalse();
        }

        [Fact]
        public void url_should_not_be_base_of_other_port()
        {
            // Given, When
            var isBaseOf = baseUri.IsCaseInsensitiveBaseOf(new Uri("http://otherhost:8080/path/path/file"));

            // Then
            isBaseOf.ShouldBeFalse();
        }

        [Fact]
        public void url_should_be_base_of_host_with_different_casing()
        {
            // Given, When
            var isBaseOf = baseUri.IsCaseInsensitiveBaseOf(new Uri("http://Host/path/path/file"));

            // Then
            isBaseOf.ShouldBeTrue();
        }

        [Fact]
        public void url_should_be_base_of_exact_path_without_trailing_slash()
        {
            // Given, When
            var isBaseOf = baseSlashUri.IsCaseInsensitiveBaseOf(new Uri("http://host/path/path"));

            // Then
            isBaseOf.ShouldBeTrue();
        }

        [Fact]
        public void url_should_be_base_of_exact_path_without_trailing_slash_with_query()
        {
            // Given, When
            var isBaseOf = baseSlashUri.IsCaseInsensitiveBaseOf(new Uri("http://host/path/path?query"));

            // Then
            isBaseOf.ShouldBeTrue();
        }

        [Fact]
        public void url_should_be_base_of_exact_path_without_trailing_slash_with_fragment()
        {
            // Given, When
            var isBaseOf = baseSlashUri.IsCaseInsensitiveBaseOf(new Uri("http://host/path/path#Fragment"));

            // Then
            isBaseOf.ShouldBeTrue();
        }

        [Fact]
        public void url_should_not_be_base_of_other_path()
        {
            // Given, When
            var isBaseOf = baseSlashUri.IsCaseInsensitiveBaseOf(new Uri("http://host/path/path2/"));

            // Then
            isBaseOf.ShouldBeFalse();
        }

        [Fact]
        public void url_should_be_base_of_same_path_with_different_host_casing()
        {
            // Given, When
            var isBaseOf = baseSlashUri.IsCaseInsensitiveBaseOf(new Uri("http://Host/path/path/"));

            // Then
            isBaseOf.ShouldBeTrue();
        }

        [Fact]
        public void url_should_be_base_of_same_path_with_different_path_casing()
        {
            // Given, When
            var isBaseOf = baseSlashUri.IsCaseInsensitiveBaseOf(new Uri("http://host/Path/PATH/"));

            // Then
            isBaseOf.ShouldBeTrue();
        }

        [Fact]
        public void url_should_be_base_of_same_path_with_different_host_using_localhost_wildcard()
        {
            // Given, When
            var isBaseOf = baseLocalHostUri.IsCaseInsensitiveBaseOf(new Uri("http://OtherHost/path/path/file"));

            // Then
            isBaseOf.ShouldBeTrue();
        }
    }
}
