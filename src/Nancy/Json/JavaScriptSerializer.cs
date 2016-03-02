//
// JavaScriptSerializer.cs
//
// Author:
//   Konstantin Triger <kostat@mainsoft.com>
//
// (C) 2007 Mainsoft, Inc.  http://www.mainsoft.com
//
//
// Permission is hereby granted, free of charge, to any person obtaining
// a copy of this software and associated documentation files (the
// "Software"), to deal in the Software without restriction, including
// without limitation the rights to use, copy, modify, merge, publish,
// distribute, sublicense, and/or sell copies of the Software, and to
// permit persons to whom the Software is furnished to do so, subject to
// the following conditions:
//
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
// LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
// OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
// WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
//

namespace Nancy.Json
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using Nancy.Json.Simple;
    using Nancy.Extensions;
    /// <summary>
    /// JavaScriptSerializer responsible for serializing objects
    /// </summary>
    public class JavaScriptSerializer
    {
        private readonly JsonConfiguration jsonConfiguration;

        private readonly GlobalizationConfiguration globalizationConfiguration;

        private readonly NancySerializationStrategy serializerStrategy;

        /// <summary>
        /// Creates an instance of <see cref="JavaScriptSerializer"/>
        /// </summary>
        public JavaScriptSerializer() : this(JsonConfiguration.Default, GlobalizationConfiguration.Default)
        {
        }

        /// <summary>
        /// Creates an instance of <see cref="JavaScriptSerializer"/>
        /// </summary>
        /// <param name="jsonConfiguration">A <see cref="JsonConfiguration"/> object to configure the serializer</param>
        /// <param name="globalizationConfiguration">A <see cref="GlobalizationConfiguration"/> object to configure the serializer</param>
        public JavaScriptSerializer(JsonConfiguration jsonConfiguration, GlobalizationConfiguration globalizationConfiguration)
        {
            this.jsonConfiguration = jsonConfiguration;
            this.globalizationConfiguration = globalizationConfiguration;
            this.serializerStrategy = new NancySerializationStrategy(jsonConfiguration.RetainCasing);
        }

        /// <summary>
        /// Creates an instance of <see cref="JavaScriptSerializer"/>
        /// </summary>
        /// <param name="jsonConfiguration">A <see cref="JsonConfiguration"/> object to configure the serializer</param>
        /// <param name="registerConverters">A boolean to determine whether to register custom converters</param>
        /// <param name="globalizationConfiguration">A <see cref="GlobalizationConfiguration"/> object to configure the serializer</param>
        public JavaScriptSerializer(JsonConfiguration jsonConfiguration, bool registerConverters, GlobalizationConfiguration globalizationConfiguration) : this(jsonConfiguration, globalizationConfiguration)
        {
            this.jsonConfiguration = jsonConfiguration;
            this.globalizationConfiguration = globalizationConfiguration;
            if (registerConverters)
            {
                this.RegisterConverters(jsonConfiguration.Converters, jsonConfiguration.PrimitiveConverters);
            }
        }

        /// <summary>
        /// Deserialize JSON
        /// </summary>
        /// <param name="input">JSON representation</param>
        /// <typeparam name="T">The <see cref="Type"/> to deserialize into</typeparam>
        /// <returns>An instance of type <typeparamref name="T"/> representing <paramref name="input"/> as an object</returns>
        public T Deserialize<T>(string input)
        {
            return SimpleJson.DeserializeObject<T>(input, this.serializerStrategy, this.globalizationConfiguration.DateTimeStyles);
        }

        /// <summary>
        /// Deserialize JSON
        /// </summary>
        /// <param name="input">JSON representation</param>
        /// <returns>An object representing <paramref name="input"/></returns>
        public object DeserializeObject(string input)
        {
            return SimpleJson.DeserializeObject(input, null, this.serializerStrategy, this.globalizationConfiguration.DateTimeStyles);
        }

        /// <summary>
        /// Register custom JSON converters
        /// </summary>
        /// <param name="converters">An array of <see cref="JavaScriptConverter"/> to register</param>
        /// <exception cref="ArgumentNullException"><paramref name="converters"/> is null</exception>
        public void RegisterConverters(IEnumerable<JavaScriptConverter> converters)
        {
            if (converters == null)
            {
                throw new ArgumentNullException("converters");
            }

            this.serializerStrategy.RegisterConverters(converters);
        }

        /// <summary>
        /// Register custom JSON converters
        /// </summary>
        /// <param name="primitiveConverters">An array of <see cref="JavaScriptPrimitiveConverter"/></param>
        /// <exception cref="ArgumentNullException"><paramref name="primitiveConverters"/> is null</exception>
        public void RegisterConverters(IEnumerable<JavaScriptPrimitiveConverter> primitiveConverters)
        {
            if (primitiveConverters == null)
            {
                throw new ArgumentNullException("primitiveConverters");
            }

            this.serializerStrategy.RegisterConverters(primitiveConverters);
        }

        /// <summary>
        /// Register custom JSON converters
        /// </summary>
        /// <param name="converters">An array of <see cref="JavaScriptConverter"/> to register</param>
        /// <param name="primitiveConverters">An array of <see cref="JavaScriptPrimitiveConverter"/></param>
        public void RegisterConverters(IEnumerable<JavaScriptConverter> converters,
            IEnumerable<JavaScriptPrimitiveConverter> primitiveConverters)
        {
            if (converters != null)
            {
                this.RegisterConverters(converters);
            }

            if (primitiveConverters != null)
            {
                this.RegisterConverters(primitiveConverters);
            }
        }

        /// <summary>
        /// Serialize an object to JSON
        /// </summary>
        /// <param name="obj">The object to serialize</param>
        /// <returns>A JSON string representation of <paramref name="obj"/></returns>
        public string Serialize(object obj)
        {
            return SimpleJson.SerializeObject(obj, this.serializerStrategy);
        }

        /// <summary>
        /// Serialize an object to JSON and write result to <paramref name="output"/>
        /// </summary>
        /// <param name="obj">The object to serialize</param>
        /// <param name="output">An instance of <see cref="TextWriter" /> to write the serialized <paramref name="obj"/></param>
        public void Serialize(object obj, TextWriter output)
        {
            output.Write(this.Serialize(obj));
        }
    }
}
