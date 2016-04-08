// --------------------------------------------------------------------------------------------------------------------
// <copyright file="BinaryStreamStack.cs" company="Jake Woods">
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
//   Provides character based and byte based stream-like read operations over multiple
//   streams and provides methods to add data to the front of the buffer.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Nancy.HttpMultipart //HttpMultipartParser
{
    /// <summary>
    ///     Provides character based and byte based stream-like read operations over multiple
    ///     streams and provides methods to add data to the front of the buffer.
    /// </summary>
    internal class BinaryStreamStack
    {
        #region Fields

        /// <summary>
        ///     Holds the streams to read from, the stream on the top of the
        ///     stack will be read first.
        /// </summary>
        private readonly Stack<BinaryReader> streams = new Stack<BinaryReader>();

        #endregion

        #region Constructors and Destructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="BinaryStreamStack" /> class with the default
        ///     encoding of UTF8.
        /// </summary>
        public BinaryStreamStack()
            : this(Encoding.UTF8)
        {
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="BinaryStreamStack" /> class.
        /// </summary>
        /// <param name="encoding">
        ///     The encoding to use for character based operations.
        /// </param>
        public BinaryStreamStack(Encoding encoding)
        {
            CurrentEncoding = encoding;
        }

        #endregion

        #region Public Properties

        /// <summary>
        ///     Gets or sets the current encoding.
        /// </summary>
        public Encoding CurrentEncoding { get; set; }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        ///     Returns true if there is any data left to read.
        /// </summary>
        /// <returns>
        ///     True or false.
        /// </returns>
        public bool HasData()
        {
            return streams.Any();
        }

        /// <summary>
        ///     Returns the reader on the top of the stack but does not remove it.
        /// </summary>
        /// <returns>
        ///     The <see cref="BinaryReader" />.
        /// </returns>
        public BinaryReader Peek()
        {
            return streams.Peek();
        }

        /// <summary>
        ///     Returns the reader on the top of the stack and removes it
        /// </summary>
        /// <returns>
        ///     The <see cref="BinaryReader" />.
        /// </returns>
        public BinaryReader Pop()
        {
            return streams.Pop();
        }

        /// <summary>
        ///     Pushes data to the front of the stack. The most recently pushed data will
        ///     be read first.
        /// </summary>
        /// <param name="data">
        ///     The data to add to the stack.
        /// </param>
        public void Push(byte[] data)
        {
            streams.Push(new BinaryReader(new MemoryStream(data), CurrentEncoding));
        }

        /// <summary>
        ///     Reads a single byte as an integer from the stack. Returns -1 if no
        ///     data is left to read.
        /// </summary>
        /// <returns>
        ///     The <see cref="byte" /> that was read.
        /// </returns>
        public int Read()
        {
            BinaryReader top = streams.Peek();

            int value;
            while ((value = top.Read()) == -1)
            {
                top.Dispose();
                streams.Pop();

                if (!streams.Any())
                {
                    return -1;
                }

                top = streams.Peek();
            }

            return value;
        }

        /// <summary>
        ///     Reads the specified number of bytes from the stack, starting from a specified point in the byte array.
        /// </summary>
        /// <param name="buffer">
        ///     The buffer to read data into.
        /// </param>
        /// <param name="index">
        ///     The index of buffer to start reading into.
        /// </param>
        /// <param name="count">
        ///     The number of bytes to read into the buffer.
        /// </param>
        /// <returns>
        ///     The number of bytes read into buffer. This might be less than the number of bytes requested if that many bytes are not available,
        ///     or it might be zero if the end of the stream is reached.
        /// </returns>
        public int Read(byte[] buffer, int index, int count)
        {
            if (!HasData())
            {
                return 0;
            }

            // Read through all the stream untill we exhaust them
            // or untill count is satisfied
            int amountRead = 0;
            BinaryReader top = streams.Peek();
            while (amountRead < count && streams.Any())
            {
                int read = top.Read(buffer, index + amountRead, count - amountRead);
                if (read == 0)
                {
                    if ((top = NextStream()) == null)
                    {
                        return amountRead;
                    }
                }
                else
                {
                    amountRead += read;
                }
            }

            return amountRead;
        }

        /// <summary>
        ///     Reads the specified number of characters from the stack, starting from a specified point in the byte array.
        /// </summary>
        /// <param name="buffer">
        ///     The buffer to read data into.
        /// </param>
        /// <param name="index">
        ///     The index of buffer to start reading into.
        /// </param>
        /// <param name="count">
        ///     The number of characters to read into the buffer.
        /// </param>
        /// <returns>
        ///     The number of characters read into buffer. This might be less than the number of bytes requested if that many bytes are not available,
        ///     or it might be zero if the end of the stream is reached.
        /// </returns>
        public int Read(char[] buffer, int index, int count)
        {
            if (!HasData())
            {
                return 0;
            }

            // Read through all the stream untill we exhaust them
            // or untill count is satisfied
            int amountRead = 0;
            BinaryReader top = streams.Peek();
            while (amountRead < count && streams.Any())
            {
                int read = top.Read(buffer, index + amountRead, count - amountRead);
                if (read == 0)
                {
                    if ((top = NextStream()) == null)
                    {
                        return amountRead;
                    }
                }
                else
                {
                    amountRead += read;
                }
            }

            return amountRead;
        }

        /// <summary>
        ///     Reads the specified number of characters from the stack, starting from a specified point in the byte array.
        /// </summary>
        /// <returns>
        ///     A byte array containing all the data up to but not including the next newline in the stack.
        /// </returns>
        public byte[] ReadByteLine()
        {
            bool dummy;
            return ReadByteLine(out dummy);
        }

        /// <summary>
        ///     Reads a line from the stack delimited by the newline for this platform. The newline
        ///     characters will not be included in the stream
        /// </summary>
        /// <param name="hitStreamEnd">
        ///     This will be set to true if we did not end on a newline but instead found the end of
        ///     our data.
        /// </param>
        /// <returns>
        ///     The <see cref="string" /> containing the line.
        /// </returns>
        public byte[] ReadByteLine(out bool hitStreamEnd)
        {
            hitStreamEnd = false;
            if (!HasData())
            {
                // No streams, no data!
                return null;
            }

            // This is horribly inefficient, consider profiling here if
            // it becomes an issue.
            BinaryReader top = streams.Peek();
            byte[] ignore = CurrentEncoding.GetBytes(new[] {'\r'});
            byte[] search = CurrentEncoding.GetBytes(new[] {'\n'});
            int searchPos = 0;
            var builder = new MemoryStream();

            while (true)
            {
                // First we need to read a byte from one of the streams
                var bytes = new byte[search.Length];
                int amountRead = top.Read(bytes, 0, bytes.Length);
                while (amountRead == 0)
                {
                    streams.Pop();
                    if (!streams.Any())
                    {
                        hitStreamEnd = true;
                        return builder.ToArray();
                    }

                    top.Dispose();
                    top = streams.Peek();
                    amountRead = top.Read(bytes, 0, bytes.Length);
                }

                // Now we've got some bytes, we need to check it against the search array.
                foreach (byte b in bytes)
                {
                    if (ignore.Contains(b))
                    {
                        continue;
                    }

                    if (b == search[searchPos])
                    {
                        searchPos += 1;
                    }
                    else
                    {
                        // We only want to append the information if it's
                        // not part of the newline sequence
                        if (searchPos != 0)
                        {
                            byte[] append = search.Take(searchPos).ToArray();
                            builder.Write(append, 0, append.Length);
                        }

                        builder.Write(new[] {b}, 0, 1);
                        searchPos = 0;
                    }

                    // Finally if we've found our string
                    if (searchPos == search.Length)
                    {
                        return builder.ToArray();
                    }
                }
            }
        }

        /// <summary>
        ///     Reads a line from the stack delimited by the newline for this platform. The newline
        ///     characters will not be included in the stream
        /// </summary>
        /// <returns>
        ///     The <see cref="string" /> containing the line.
        /// </returns>
        public string ReadLine()
        {
            bool dummy;
            return ReadLine(out dummy);
        }

        /// <summary>
        ///     Reads a line from the stack delimited by the newline for this platform. The newline
        ///     characters will not be included in the stream
        /// </summary>
        /// <param name="hitStreamEnd">
        ///     This will be set to true if we did not end on a newline but instead found the end of
        ///     our data.
        /// </param>
        /// <returns>
        ///     The <see cref="string" /> containing the line.
        /// </returns>
        public string ReadLine(out bool hitStreamEnd)
        {
            bool foundEnd;
            byte[] result = ReadByteLine(out foundEnd);
            hitStreamEnd = foundEnd;

            if (result == null)
            {
                return null;
            }

            return CurrentEncoding.GetString(result);
        }

        #endregion

        #region Methods

        /// <summary>
        ///     Removes the current reader from the stack and ensures it is correctly
        ///     destroyed and then returns the next available reader. If no reader
        ///     is available this method returns null.
        /// </summary>
        /// <returns>
        ///     The next <see cref="BinaryReader">reader</see>.
        /// </returns>
        private BinaryReader NextStream()
        {
            BinaryReader top = streams.Pop();
            top.Dispose();

            return streams.Any() ? streams.Peek() : null;
        }

        #endregion
    }
}