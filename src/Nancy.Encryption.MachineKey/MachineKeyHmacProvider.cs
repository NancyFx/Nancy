namespace Nancy.Encryption.MachineKey
{
    using System;
    using System.Configuration;
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
        public int HmacLength { get; private set; }

        public MachineKeyHmacProvider()
        {
            this.GetHmacLength();
        }

        public byte[] GenerateHmac(string data)
        {
            var input = Encoding.UTF8.GetBytes(data);

            return this.GenerateHmac(input);
        }

        public byte[] GenerateHmac(byte[] data)
        {
            var encoded = MachineKey.Encode(data, MachineKeyProtection.Validation);

            var bytes = HexStringToByteArray(encoded);

            return bytes.Skip(bytes.Length - HmacLength).ToArray();
        }

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

        public static byte[] HexStringToByteArray(string data)
        {
            if (string.IsNullOrEmpty(data) || data.Length % 2 != 0)
            {
                return new byte[] { };
            }

            var output = new byte[data.Length / 2];

            for (var i = 0; i < data.Length / 2; i++)
            {
                var numberString = new string(new[] { data[2 * i], data[2 * i + 1] });

                var value = byte.Parse(numberString, System.Globalization.NumberStyles.HexNumber);

                output[i] = value;
            }

            return output;
        }
    }
}