// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MultipartFormDataParser.cs" company="Jake Woods">
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
// --------------------------------------------------------------------------------------------------------------------

using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Linq;

namespace Nancy.HttpMultipart
{
    /// <summary>
    ///     Provides methods to parse a
    ///     <see href="http://www.ietf.org/rfc/rfc2388.txt">
    ///         <c>multipart/form-data</c>
    ///     </see>
    ///     stream into it's parameters and file data.
    /// </summary>
    /// <remarks>
    ///     <para>
    ///         A parameter is defined as any non-file data passed in the multipart stream. For example
    ///         any form fields would be considered a parameter.
    ///     </para>
    ///     <para>
    ///         The parser determines if a section is a file or not based on the presence or absence
    ///         of the filename argument for the Content-Type header. If filename is set then the section
    ///         is assumed to be a file, otherwise it is assumed to be parameter data.
    ///     </para>
    /// </remarks>
    /// <example>
    ///     <code lang="C#"> 
    ///       Stream multipartStream = GetTheMultipartStream();
    ///       string boundary = GetTheBoundary();
    ///       var parser = new MultipartFormDataParser(multipartStream, boundary, Encoding.UTF8);
    ///  
    ///       // Grab the parameters (non-file data). Key is based on the name field
    ///       var username = parser.Parameters["username"].Data;
    ///       var password = parser.parameters["password"].Data;
    ///       
    ///       // Grab the first files data
    ///       var file = parser.Files.First();
    ///       var filename = file.FileName;
    ///       var filestream = file.Data;
    ///   </code>
    ///     <code lang="C#">
    ///     // In the context of WCF you can get the boundary from the HTTP
    ///     // request
    ///     public ResponseClass MyMethod(Stream multipartData)
    ///     {
    ///         // First we need to get the boundary from the header, this is sent
    ///         // with the HTTP request. We can do that in WCF using the WebOperationConext:
    ///         var type = WebOperationContext.Current.IncomingRequest.Headers["Content-Type"];
    /// 
    ///         // Now we want to strip the boundary out of the Content-Type, currently the string
    ///         // looks like: "multipart/form-data; boundary=---------------------124123qase124"
    ///         var boundary = type.Substring(type.IndexOf('=')+1);
    /// 
    ///         // Now that we've got the boundary we can parse our multipart and use it as normal
    ///         var parser = new MultipartFormDataParser(data, boundary, Encoding.UTF8);
    /// 
    ///         ...
    ///     }
    ///   </code>
    /// </example>
    public class MultipartFormDataParser
    {
        #region Constants

        /// <summary>
        ///     The default buffer size.
        /// </summary>
        private const int DefaultBufferSize = 4096;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="MultipartFormDataParser" /> class
        ///     with an input stream. Boundary will be automatically detected based on the
        ///     first line of input.
        /// </summary>
        /// <param name="stream">
        ///     The stream containing the multipart data
        /// </param>
        public MultipartFormDataParser(Stream stream)
            : this(stream, null, Encoding.UTF8, DefaultBufferSize)
        {
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="MultipartFormDataParser" /> class
        ///     with the boundary and input stream.
        /// </summary>
        /// <param name="stream">
        ///     The stream containing the multipart data
        /// </param>
        /// <param name="boundary">
        ///     The multipart/form-data boundary. This should be the value
        ///     returned by the request header.
        /// </param>
        public MultipartFormDataParser(Stream stream, string boundary)
            : this(stream, boundary, Encoding.UTF8, DefaultBufferSize)
        {
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="MultipartFormDataParser" /> class
        ///     with the input stream and stream encoding. Boundary is automatically
        ///     detected.
        /// </summary>
        /// <param name="stream">
        ///     The stream containing the multipart data
        /// </param>
        /// <param name="encoding">
        ///     The encoding of the multipart data
        /// </param>
        public MultipartFormDataParser(Stream stream, Encoding encoding)
            : this(stream, null, encoding, DefaultBufferSize)
        {
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="MultipartFormDataParser" /> class
        ///     with the boundary, input stream and stream encoding.
        /// </summary>
        /// <param name="stream">
        ///     The stream containing the multipart data
        /// </param>
        /// <param name="boundary">
        ///     The multipart/form-data boundary. This should be the value
        ///     returned by the request header.
        /// </param>
        /// <param name="encoding">
        ///     The encoding of the multipart data
        /// </param>
        public MultipartFormDataParser(Stream stream, string boundary, Encoding encoding)
            : this(stream, boundary, encoding, DefaultBufferSize)
        {
            // 4096 is the optimal buffer size as it matches the internal buffer of a StreamReader
            // See: http://stackoverflow.com/a/129318/203133
            // See: http://msdn.microsoft.com/en-us/library/9kstw824.aspx (under remarks)
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="MultipartFormDataParser" /> class
        ///     with the stream, input encoding and buffer size. Boundary is automatically
        ///     detected.
        /// </summary>
        /// <param name="stream">
        ///     The stream containing the multipart data
        /// </param>
        /// <param name="encoding">
        ///     The encoding of the multipart data
        /// </param>
        /// <param name="binaryBufferSize">
        ///     The size of the buffer to use for parsing the multipart form data. This must be larger
        ///     then (size of boundary + 4 + # bytes in newline).
        /// </param>
        public MultipartFormDataParser(Stream stream, Encoding encoding, int binaryBufferSize)
            : this(stream, null, encoding, binaryBufferSize)
        {
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="MultipartFormDataParser" /> class
        ///     with the boundary, stream, input encoding and buffer size.
        /// </summary>
        /// <param name="stream">
        ///     The stream containing the multipart data
        /// </param>
        /// <param name="boundary">
        ///     The multipart/form-data boundary. This should be the value
        ///     returned by the request header.
        /// </param>
        /// <param name="encoding">
        ///     The encoding of the multipart data
        /// </param>
        /// <param name="binaryBufferSize">
        ///     The size of the buffer to use for parsing the multipart form data. This must be larger
        ///     then (size of boundary + 4 + # bytes in newline).
        /// </param>
        public MultipartFormDataParser(Stream stream, string boundary, Encoding encoding, int binaryBufferSize)
        {
            Files = new List<FilePart>();
            Parameters = new List<ParameterPart>();

            var streamingParser = new StreamingMultipartFormDataParser(stream, boundary, encoding, binaryBufferSize);
            streamingParser.ParameterHandler += parameterPart => Parameters.Add(parameterPart);

            streamingParser.FileHandler += (name, fileName, type, disposition, buffer, bytes) =>
                {
                    if (Files.Count == 0 || name != Files[Files.Count - 1].Name)
                    {
                        Files.Add(new FilePart(name, fileName, new MemoryStream(), type, disposition));
                    }

                    Files[Files.Count - 1].Data.Write(buffer, 0, bytes);
                };

            streamingParser.Run();

            // Reset all the written memory streams so they can be read.
            foreach (var file in Files)
            {
                file.Data.Position = 0;
            }
        }

        #endregion

        #region Public Properties

        /// <summary>
        ///     Gets the mapping of parameters parsed files. The name of a given field
        ///     maps to the parsed file data.
        /// </summary>
        public List<FilePart> Files { get; private set; }

        /// <summary>
        ///     Gets the parameters. Several ParameterParts may share the same name.
        /// </summary>
        public List<ParameterPart> Parameters { get; private set; }

        #endregion

        #region Public Methods

        /// <summary>
        /// Returns true if the parameter has any values. False otherwise
        /// </summary>
        /// <param name="name">The name of the parameter</param>
        /// <returns>True if the parameter exists. False otherwise</returns>
        public bool HasParameter(string name)
        {
            return Parameters.Any(p => p.Name == name);
        }

        /// <summary>
        /// Returns the value of a parameter or null if it doesn't exist. 
        /// 
        /// You should only use this method if you're sure the parameter has only one value. 
        /// 
        /// If you need to support multiple values use GetParameterValues.
        /// </summary>
        /// <param name="name">The name of the parameter</param>
        /// <returns>The value of the parameter</returns>
        public string GetParameterValue(string name)
        {
            return Parameters.FirstOrDefault(p => p.Name == name).Data;
        }

        /// <summary>
        /// Returns the values of a parameter or an empty enumerable if the parameter doesn't exist.
        /// </summary>
        /// <param name="name">The name of the parameter</param>
        /// <returns>The values of the parameter</returns>
        public IEnumerable<string> GetParameterValues(string name)
        {
            return Parameters
                .Where(p => p.Name == name)
                .Select(p => p.Data);
        }

        #endregion
    }
}