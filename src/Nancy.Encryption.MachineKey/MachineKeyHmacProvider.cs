namespace Nancy.Encryption.MachineKey
{
    using System;
    using System.Configuration;
    using System.Globalization;
    using System.Linq;
    using System.Reflection;
    using System.Text;
    using System.Web.Configuration;
    using System.Web.Security;

    using Nancy.Cryptography;

    /// <summary>
    /// A HMAC provider using the ASP.Net Machine key
    /// 
    /// This is hacky as anything because of all kinds of horrible
    /// internal sealed nonsense inside the framework.
    /// </summary>
    public class MachineKeyHmacProvider : IHmacProvider
    {
        /// <summary>
        /// Gets the length of the HMAC signature in bytes
        /// </summary>
        public int HmacLength { get; private set; }

        public MachineKeyHmacProvider()
        {
            this.GetHmacLength();
        }

        /// <summary>
        /// Create a hmac from the given data
        /// </summary>
        /// <param name="data">Data to create hmac from</param>
        /// <returns>Hmac bytes</returns>
        public byte[] GenerateHmac(string data)
        {
            var input = Encoding.UTF8.GetBytes(data);

            return this.GenerateHmac(input);
        }

        /// <summary>
        /// Create a hmac from the given data
        /// </summary>
        /// <param name="data">Data to create hmac from</param>
        /// <returns>Hmac bytes</returns>
        public byte[] GenerateHmac(byte[] data)
        {
            var encoded = MachineKey.Encode(data, MachineKeyProtection.Validation);

            var bytes = HexStringToByteArray(encoded);

            return bytes.Skip(bytes.Length - HmacLength).ToArray();
        }

        /// <summary>
        /// Uses reflection to get the hmac length that machine key will generate
        /// </summary>
        private void GetHmacLength()
        {
            // Yucky reflection because it doesn't expose this for some reason :(
            var machineKeyConfig = (MachineKeySection)ConfigurationManager.GetSection("system.web/machineKey");
            var machineKeySectionType = machineKeyConfig.GetType();
            var field = machineKeySectionType.GetField("_HashSize", BindingFlags.NonPublic | BindingFlags.Static);

            try
            {
                this.HmacLength = (int)field.GetValue(null);
            }
            catch (Exception)
            {
                throw new InvalidOperationException("Unable to retrieve hash size from machine.config");
            }
        }

        /// <summary>
        /// Converts a string of "hex bytes" to actual bytes.
        /// We could just use the same method as .net does but
        /// like a lot of useful things in .net, it's internal.
        /// </summary>
        /// <param name="data">String of hex bytes</param>
        /// <returns>Actual byte array</returns>
        private static byte[] HexStringToByteArray(string data)
        {
            if (string.IsNullOrEmpty(data) || data.Length % 2 != 0)
            {
                return new byte[] { };
            }

            var output = new byte[data.Length / 2];

            for (var i = 0; i < data.Length / 2; i++)
            {
                var numberString = new string(new[] { data[2 * i], data[2 * i + 1] });

                var value = byte.Parse(numberString, NumberStyles.HexNumber);

                output[i] = value;
            }

            return output;
        }
    }
}