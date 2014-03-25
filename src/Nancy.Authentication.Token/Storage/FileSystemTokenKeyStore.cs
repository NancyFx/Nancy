namespace Nancy.Authentication.Token.Storage
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Runtime.Serialization.Formatters.Binary;

    /// <summary>
    /// Stores encryption keys in the file system
    /// </summary>
    public class FileSystemTokenKeyStore : ITokenKeyStore
    {
        private IRootPathProvider rootPathProvider;

        private BinaryFormatter binaryFormatter;

        private static object syncLock = new object();

        /// <summary>
        /// Creates a new <see cref="FileSystemTokenKeyStore"/>
        /// </summary>
        public FileSystemTokenKeyStore()
            : this(new DefaultRootPathProvider())
        {
        }

        /// <summary>
        /// Creates a new <see cref="FileSystemTokenKeyStore"/>
        /// </summary>
        /// <param name="rootPathProvider"></param>
        public FileSystemTokenKeyStore(IRootPathProvider rootPathProvider)
        {
            this.rootPathProvider = rootPathProvider;
            this.binaryFormatter = new BinaryFormatter();
        }

        /// <summary>
        /// Retrieves encryption keys.
        /// </summary>
        /// <returns>Keys</returns>
        public IDictionary<DateTime, byte[]> Retrieve()
        {
            lock (syncLock)
            {
                if (!File.Exists(FilePath))
                {
                    return new Dictionary<DateTime, byte[]>();
                }

                using (var fileStream = new FileStream(FilePath, FileMode.Open, FileAccess.Read, FileShare.None))
                {
                    return (Dictionary<DateTime, byte[]>)binaryFormatter.Deserialize(fileStream);
                }
            }
        }

        /// <summary>
        /// Stores encyrption keys.
        /// </summary>
        /// <param name="keys">Keys</param>
        public void Store(IDictionary<DateTime, byte[]> keys)
        {
            lock (syncLock)
            {
                if (!Directory.Exists(StorageLocation))
                {
                    Directory.CreateDirectory(StorageLocation);
                }

                var keyChain = new Dictionary<DateTime, byte[]>(keys);

                using (var fileStream = new FileStream(FilePath, FileMode.OpenOrCreate, FileAccess.Write, FileShare.None))
                {
                    binaryFormatter.Serialize(fileStream, keyChain);
                }
            }
        }

        /// <summary>
        /// Purges encryption keys
        /// </summary>
        public void Purge()
        {
            if (File.Exists(FilePath))
            {
                File.Delete(FilePath);
            }
            if (Directory.Exists(StorageLocation))
            {
                Directory.Delete(StorageLocation);
            }
        }

        /// <summary>
        /// The location where token keys are stored
        /// </summary>
        public string FilePath
        {
            get
            {
                return Path.Combine(StorageLocation, "keyChain.bin");
            }
        }

        private string StorageLocation
        {
            get
            {
                return Path.Combine(rootPathProvider.GetRootPath(), "keyStore");
            }
        }
    }
}