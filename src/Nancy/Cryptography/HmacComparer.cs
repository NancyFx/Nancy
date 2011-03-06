namespace Nancy.Cryptography
{
    using System;
    using System.Runtime.CompilerServices;

    public static class HmacComparer
    {
        /// <summary>
        /// Compare two hmac byte arrays without any early exits
        /// </summary>
        /// <param name="hmac1">First hmac</param>
        /// <param name="hmac2">Second hmac</param>
        /// <param name="hashLength">Expected length of the hash</param>
        /// <returns>True if equal, false otherwise</returns>
        [MethodImpl(MethodImplOptions.NoInlining | MethodImplOptions.NoOptimization)]
        public static bool Compare(byte[] hmac1, byte[] hmac2, int hashLength)
        {
            var hasResized = false;

            if (hmac1.Length != hashLength)
            {
                Array.Resize(ref hmac1, hashLength);
                hasResized = true;
            }

            if (hmac2.Length != hashLength)
            {
                Array.Resize(ref hmac2, hashLength);
                hasResized = true;
            }

            var isValid = true;
            for (int i = 0; i < hashLength; i++)
            {
                if (hmac1[i] != hmac2[i])
                {
                    isValid = false;
                }
            }

            return hasResized ? false : isValid;
        }
    }
}