using System;

namespace Nancy.Authentication.Token.Tests.Storage
{
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text;
    using Nancy.Authentication.Token.Storage;
    using Nancy.Testing.Fakes;
    using Xunit;

    public class FileSystemTokenKeyStoreFixture : IDisposable
    {
        private FileSystemTokenKeyStore keyStore;

        public FileSystemTokenKeyStoreFixture()
        {
            var rootPathProvider = new FakeRootPathProvider();
            this.keyStore = new FileSystemTokenKeyStore(rootPathProvider);
        }

        [Fact]
        public void Should_store_keys_in_file()
        {
            var keys = new Dictionary<DateTime, byte[]>();

            keys.Add(DateTime.UtcNow, Encoding.UTF8.GetBytes("fake encryption key"));

            keyStore.Store(keys);

            Assert.True(File.Exists(keyStore.FilePath));
        }

        [Fact]
        public void Should_retrieve_keys_from_file()
        {
            var keys = new Dictionary<DateTime, byte[]>();

            keys.Add(DateTime.UtcNow, Encoding.UTF8.GetBytes("fake encryption key"));

            keyStore.Store(keys);

            var retrievedKeys = keyStore.Retrieve();

            Assert.True(Encoding.UTF8.GetString(retrievedKeys.Values.First()) 
                == "fake encryption key");
        }

        public void Dispose()
        {
            keyStore.Purge();
        }
    }
}
