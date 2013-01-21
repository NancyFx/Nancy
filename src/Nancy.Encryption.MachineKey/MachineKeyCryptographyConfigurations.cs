namespace Nancy.Encryption.MachineKey
{
    using System;

    using Nancy.Cryptography;

    /// <summary>
    /// Helpers for creating crypto configs from machine.config crypto types
    /// </summary>
    public static class MachineKeyCryptographyConfigurations
    {
        private static readonly Lazy<CryptographyConfiguration> DefaultConfiguration =
            new Lazy<CryptographyConfiguration>(() => new CryptographyConfiguration(
                                                          new MachineKeyEncryptionProvider(),
                                                          new MachineKeyHmacProvider()));

        private static readonly Lazy<CryptographyConfiguration> NoEncryptionConfiguration =
            new Lazy<CryptographyConfiguration>(() => new CryptographyConfiguration(
                                                          new NoEncryptionProvider(),
                                                          new MachineKeyHmacProvider()));

        /// <summary>
        /// Gets the default configuration for machinekey encryption.
        /// Uses the machine key for both encryption and hmac generation
        /// </summary>
        public static CryptographyConfiguration Default
        {
            get
            {
                return DefaultConfiguration.Value;
            }
        }

        /// <summary>
        /// Gets the configuration to use machine key for HMAC, but no encryption
        /// </summary>
        public static CryptographyConfiguration NoEncryption
        {
            get
            {
                return NoEncryptionConfiguration.Value;
            }
        }
    }
}