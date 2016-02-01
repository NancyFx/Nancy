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

    public class JavaScriptSerializer
    {
        internal const string SerializedTypeNameKey = "__type";

        int _maxJsonLength;
        int _recursionLimit;
        bool _retainCasing;
        bool _iso8601DateFormat;

        readonly NancySerializationStrategy _serializerStrategy;

#if NET_3_5
        internal static readonly JavaScriptSerializer DefaultSerializer = new JavaScriptSerializer(false, 2097152, 100);

        public JavaScriptSerializer()
            : this(null, false, 2097152, 100)
        {
        }
#else
        internal static readonly JavaScriptSerializer DefaultSerializer = new JavaScriptSerializer(false, 102400, 100, false, true, null, null);

        public JavaScriptSerializer()
            : this(false, 102400, 100, false, true, null, null)
        {
        }

#endif
        public JavaScriptSerializer(bool registerConverters, int maxJsonLength, int recursionLimit, bool retainCasing, bool iso8601DateFormat, IEnumerable<JavaScriptConverter> converters, IEnumerable<JavaScriptPrimitiveConverter> primitiveConverters)
        {
            _serializerStrategy = new NancySerializationStrategy(retainCasing, registerConverters, iso8601DateFormat, converters, primitiveConverters);
            _maxJsonLength = maxJsonLength;
            _recursionLimit = recursionLimit;

            this.RetainCasing = retainCasing;

            _iso8601DateFormat = iso8601DateFormat;

            if (registerConverters)
                RegisterConverters(converters, primitiveConverters);

        }


        public int MaxJsonLength
        {
            get
            {
                return _maxJsonLength;
            }
            set
            {
                _maxJsonLength = value;
            }
        }

        public int RecursionLimit
        {
            get
            {
                return _recursionLimit;
            }
            set
            {
                _recursionLimit = value;
            }
        }

        public bool ISO8601DateFormat
        {
            get { return _iso8601DateFormat; }
            set { _iso8601DateFormat = value; }
        }

        public bool RetainCasing
        {
            get { return this._retainCasing; }
            set { this._retainCasing = value; }
        }

        public T Deserialize<T>(string input)
        {
            return SimpleJson.DeserializeObject<T>(input, _serializerStrategy);
        }

        public object DeserializeObject(string input)
        {
            return SimpleJson.DeserializeObject(input, null, _serializerStrategy);
        }

        public void RegisterConverters(IEnumerable<JavaScriptConverter> converters)
        {
            if (converters == null)
                throw new ArgumentNullException("converters");

            _serializerStrategy.RegisterConverters(converters);
        }

        public void RegisterConverters(IEnumerable<JavaScriptPrimitiveConverter> primitiveConverters)
        {
            if (primitiveConverters == null)
                throw new ArgumentNullException("primitiveConverters");

            _serializerStrategy.RegisterConverters(primitiveConverters);
        }

        public void RegisterConverters(IEnumerable<JavaScriptConverter> converters, IEnumerable<JavaScriptPrimitiveConverter> primitiveConverters)
        {
            if (converters != null)
                RegisterConverters(converters);

            if (primitiveConverters != null)
                RegisterConverters(primitiveConverters);
        }

        public string Serialize(object obj)
        {
            return SimpleJson.SerializeObject(obj, _serializerStrategy);
        }

        internal void Serialize(object obj, TextWriter output)
        {
            output.Write(Serialize(obj));
        }
    }
}
