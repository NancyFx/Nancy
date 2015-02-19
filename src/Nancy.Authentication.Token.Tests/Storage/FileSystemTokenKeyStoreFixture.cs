namespace Nancy.Authentication.Token.Tests.Storage
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text;

    using Nancy.Authentication.Token.Storage;
    using Nancy.Testing.Fakes;

    using Xunit;

    public class FileSystemTokenKeyStoreFixture
    {
        [Fact]
        public void Should_store_keys_in_file()
        {
            // Given
            var keyStore = GetKeyStore();
            try
            {
                var keys = new Dictionary<DateTime, byte[]>
                {
                    { DateTime.UtcNow, Encoding.UTF8.GetBytes("fake encryption key") }
                };

                // When
                keyStore.Store(keys);

                // Then
                Assert.True(File.Exists(keyStore.FilePath));
            }
            finally
            {
                keyStore.Purge();
            }
        }

        [Fact]
        public void Should_retrieve_keys_from_file()
        {
            // Given
            var keyStore = GetKeyStore();
            try
            {
                var keys = new Dictionary<DateTime, byte[]>
                {
                    { DateTime.UtcNow, Encoding.UTF8.GetBytes("fake encryption key") }
                };

                keyStore.Store(keys);

                // When
                var retrievedKeys = keyStore.Retrieve();

                // Then
                Assert.True(Encoding.UTF8.GetString(retrievedKeys.Values.First()) == "fake encryption key");
            }
            finally
            {
                keyStore.Purge();
            }
        }

        private static FileSystemTokenKeyStore GetKeyStore()
        {
            var rootPathProvider = new FakeRootPathProvider();
            return new FileSystemTokenKeyStore(rootPathProvider);
        }
    }
}
