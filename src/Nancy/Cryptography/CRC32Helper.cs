using System;
using System.Collections;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace Nancy.Cryptography
{
    /// Thanks to Tamir Khason
    /// <summary>Implements a 32-bits cyclic redundancy check (CRC) hash algorithm.</summary>
    /// <remarks>This class is not intended to be used for security purposes. For security applications use MD5, SHA1, SHA256, SHA384, 
    /// or SHA512 in the System.Security.Cryptography namespace.</remarks>
    public class CRC32Helper : HashAlgorithm
    {

        #region CONSTRUCTORS
        /// <summary>Creates a CRC32 object using the <see cref="DefaultPolynomial"/>.</summary>
        public CRC32Helper()
            : this(DefaultPolynomial)
        {
        }

        /// <summary>Creates a CRC32 object using the specified polynomial.</summary>
        /// <remarks>The polynomical should be supplied in its bit-reflected form. <see cref="DefaultPolynomial"/>.</remarks>
        [CLSCompliant(false)]
        public CRC32Helper(uint polynomial)
        {
            HashSizeValue = 32;
            _crc32Table = (uint[])_crc32TablesCache[polynomial];
            if (_crc32Table == null)
            {
                _crc32Table = CRC32Helper._buildCRC32Table(polynomial);
                _crc32TablesCache.Add(polynomial, _crc32Table);
            }
            Initialize();
        }

        // static constructor
        static CRC32Helper()
        {
            _crc32TablesCache = Hashtable.Synchronized(new Hashtable());
            _defaultCRC = new CRC32Helper();
        }
        #endregion

        #region PROPERTIES
        /// <summary>Gets the default polynomial (used in WinZip, Ethernet, etc.)</summary>
        /// <remarks>The default polynomial is a bit-reflected version of the standard polynomial 0x04C11DB7 used by WinZip, Ethernet, etc.</remarks>
        [CLSCompliant(false)]
        public static readonly uint DefaultPolynomial = 0xEDB88320; // Bitwise reflection of 0x04C11DB7;
        #endregion

        #region METHODS
        /// <summary>Initializes an implementation of HashAlgorithm.</summary>
        public override void Initialize()
        {
            _crc = _allOnes;
        }

        /// <summary>Routes data written to the object into the hash algorithm for computing the hash.</summary>
        protected override void HashCore(byte[] buffer, int offset, int count)
        {
            for (int i = offset; i < count; i++)
            {
                ulong ptr = (_crc & 0xFF) ^ buffer[i];
                _crc >>= 8;
                _crc ^= _crc32Table[ptr];
            }
        }

        /// <summary>Finalizes the hash computation after the last data is processed by the cryptographic stream object.</summary>
        protected override byte[] HashFinal()
        {
            byte[] finalHash = new byte[4];
            ulong finalCRC = _crc ^ _allOnes;

            finalHash[0] = (byte)((finalCRC >> 0) & 0xFF);
            finalHash[1] = (byte)((finalCRC >> 8) & 0xFF);
            finalHash[2] = (byte)((finalCRC >> 16) & 0xFF);
            finalHash[3] = (byte)((finalCRC >> 24) & 0xFF);

            return finalHash;
        }

        /// <summary>Computes the CRC32 value for the given ASCII string using the <see cref="DefaultPolynomial"/>.</summary>
        public static int Compute(string asciiString)
        {
            _defaultCRC.Initialize();
            return ToInt32(_defaultCRC.ComputeHash(asciiString));
        }

        /// <summary>Computes the CRC32 value for the given input stream using the <see cref="DefaultPolynomial"/>.</summary>
        public static int Compute(Stream inputStream)
        {
            _defaultCRC.Initialize();
            return ToInt32(_defaultCRC.ComputeHash(inputStream));
        }

        /// <summary>Computes the CRC32 value for the input data using the <see cref="DefaultPolynomial"/>.</summary>
        public static int Compute(byte[] buffer)
        {
            _defaultCRC.Initialize();
            return ToInt32(_defaultCRC.ComputeHash(buffer));
        }

        /// <summary>Computes the hash value for the input data using the <see cref="DefaultPolynomial"/>.</summary>
        public static int Compute(byte[] buffer, int offset, int count)
        {
            _defaultCRC.Initialize();
            return ToInt32(_defaultCRC.ComputeHash(buffer, offset, count));
        }

        /// <summary>Computes the hash value for the given ASCII string.</summary>
        /// <remarks>The computation preserves the internal state between the calls, so it can be used for computation of a stream data.</remarks>
        public byte[] ComputeHash(string asciiString)
        {
            byte[] rawBytes = ASCIIEncoding.ASCII.GetBytes(asciiString);
            return ComputeHash(rawBytes);
        }

        /// <summary>Computes the hash value for the given input stream.</summary>
        /// <remarks>The computation preserves the internal state between the calls, so it can be used for computation of a stream data.</remarks>
        new public byte[] ComputeHash(Stream inputStream)
        {
            byte[] buffer = new byte[4096];
            int bytesRead;
            while ((bytesRead = inputStream.Read(buffer, 0, 4096)) > 0)
            {
                HashCore(buffer, 0, bytesRead);
            }
            return HashFinal();
        }

        /// <summary>Computes the hash value for the input data.</summary>
        /// <remarks>The computation preserves the internal state between the calls, so it can be used for computation of a stream data.</remarks>
        new public byte[] ComputeHash(byte[] buffer)
        {
            return ComputeHash(buffer, 0, buffer.Length);
        }

        /// <summary>Computes the hash value for the input data.</summary>
        /// <remarks>The computation preserves the internal state between the calls, so it can be used for computation of a stream data.</remarks>
        new public byte[] ComputeHash(byte[] buffer, int offset, int count)
        {
            HashCore(buffer, offset, count);
            return HashFinal();
        }
        #endregion

        #region PRIVATE SECTION
        private static uint _allOnes = 0xffffffff;
        private static CRC32Helper _defaultCRC;
        private static Hashtable _crc32TablesCache;
        private uint[] _crc32Table;
        private uint _crc;

        // Builds a crc32 table given a polynomial
        private static uint[] _buildCRC32Table(uint polynomial)
        {
            uint crc;
            uint[] table = new uint[256];

            // 256 values representing ASCII character codes. 
            for (int i = 0; i < 256; i++)
            {
                crc = (uint)i;
                for (int j = 8; j > 0; j--)
                {
                    if ((crc & 1) == 1)
                        crc = (crc >> 1) ^ polynomial;
                    else
                        crc >>= 1;
                }
                table[i] = crc;
            }

            return table;
        }

        private static int ToInt32(byte[] buffer)
        {
            return BitConverter.ToInt32(buffer, 0);
        }
        #endregion

    }
}