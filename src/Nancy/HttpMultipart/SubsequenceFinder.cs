// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SubsequenceFinder.cs" company="Jake Woods">
//   Copyright (c) 2013 Jake Woods
//   
//   Permission is hereby granted, free of charge, to any person obtaining a copy of this software 
//   and associated documentation files (the "Software"), to deal in the Software without restriction, 
//   including without limitation the rights to use, copy, modify, merge, publish, distribute, 
//   sublicense, and/or sell copies of the Software, and to permit persons to whom the Software 
//   is furnished to do so, subject to the following conditions:
//   
//   The above copyright notice and this permission notice shall be included in all copies 
//   or substantial portions of the Software.
//   
//   THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, 
//   INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR 
//   PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR 
//   ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, 
//   ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
// </copyright>
// <author>Jake Woods</author>
// <summary>
//   Provides methods to find a subsequence within a
//   sequence.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System.Collections.Generic;
using System.Linq;

namespace Nancy.HttpMultipart
{
    /// <summary>
    ///     Provides methods to find a subsequence within a
    ///     sequence.
    /// </summary>
    internal class SubsequenceFinder
    {
        #region Public Methods and Operators

        public static int Search(byte[] haystack, byte[] needle)
        {
            return Search(haystack, needle, haystack.Length);
        }

        /// <summary>
        ///     Finds if a sequence exists within another sequence. 
        /// </summary>
        /// <param name="haystack">
        ///     The sequence to search
        /// </param>
        /// <param name="needle">
        ///     The sequence to look for
        /// </param>
        /// <param name="haystackLength">
        ///     The length of the haystack to consider for searching
        /// </param>
        /// <returns>
        ///     The start position of the found sequence or -1 if nothing was found
        /// </returns>
        public static int Search(byte[] haystack, byte[] needle, int haystackLength)
        {
            var charactersInNeedle = new HashSet<byte>(needle);

            var length = needle.Length;
            var index = 0;
            while (index + length <= haystackLength)
            {
                // Worst case scenario: Go back to character-by-character parsing until we find a non-match
                // or we find the needle.
                if (charactersInNeedle.Contains(haystack[index + length - 1]))
                {
                    var needleIndex = 0;
                    while (haystack[index + needleIndex] == needle[needleIndex])
                    {
                        if (needleIndex == needle.Length - 1)
                        {
                            // Found our match!
                            return index;
                        }

                        needleIndex += 1;
                    }

                    index += 1;
                    index += needleIndex;
                    continue;
                }

                index += length;
            }

            return -1;
        }

        #endregion
    }
}