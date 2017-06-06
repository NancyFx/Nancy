namespace Nancy.Prototype.Http
{
    using System;
    using System.Collections;

    internal static class HttpUtility
    {
        private const string Token =
            "!#$%&'*+-.0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ^_`abcdefghijklmnopqrstuvwxyz|~";

        private static readonly Lazy<TokenValues> Values =
            new Lazy<TokenValues>(() => GetTokenValues(Token.ToCharArray()));

        public static bool IsValidToken(string value)
        {
            Check.NotNull(value, nameof(value));

            var values = Values.Value;

            foreach (var @char in value)
            {
                if (!values.IsValid(@char))
                {
                    return false;
                }
            }

            return true;
        }

        private static TokenValues GetTokenValues(char[] validValues)
        {
            int minValue = validValues[0];
            int maxValue = validValues[validValues.Length - 1];

            var length = maxValue - minValue + 1;

            var bitArray = new BitArray(length);

            foreach (var value in validValues)
            {
                bitArray.Set(value - minValue, true);
            }

            return new TokenValues(bitArray, minValue);
        }

        private struct TokenValues
        {
            private readonly BitArray bitArray;

            private readonly int offset;

            public TokenValues(BitArray bitArray, int offset)
            {
                this.bitArray = bitArray;
                this.offset = offset;
            }

            public bool IsValid(char value)
            {
                var index = value - this.offset;

                return index >= 0
                    && index < this.bitArray.Length
                    && this.bitArray[index];
            }
        }
    }
}
