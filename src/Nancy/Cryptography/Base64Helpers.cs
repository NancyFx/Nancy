namespace Nancy.Cryptography
{
    using System;

    public static class Base64Helpers
    {
        /// <summary>
        /// Calculates how long a byte array of X length will be after base64 encoding
        /// </summary>
        /// <param name="normalLength">The normal, 8bit per byte, length of the byte array</param>
        /// <returns>Base64 encoded length</returns>
        public static int GetBase64Length(int normalLength)
        {
            var inputPadding = (normalLength % 3 != 0) ? (3 - (normalLength % 3)) : 0;

            return (int)Math.Ceiling((normalLength + inputPadding) * 4.0 / 3.0);
        }
    }
}