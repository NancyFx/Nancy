//
// JsonSerializer.cs
//
// Author:
//   Marek Habersack <mhabersack@novell.com>
//
// (C) 2008 Novell, Inc.  http://novell.com/
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
    using System.Collections;
    using System.Collections.Generic;
    using System.Globalization;
    using System.IO;
    using System.Reflection;
    using System.Text;

    internal sealed class JsonSerializer
    {
        internal static readonly long InitialJavaScriptDateTicks = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc).Ticks;
        private static readonly DateTime MinimumJavaScriptDate = new DateTime(100, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        private static readonly MethodInfo serializeGenericDictionary = typeof (JsonSerializer).GetMethod("SerializeGenericDictionary",
            BindingFlags.NonPublic | BindingFlags.Instance);

        private readonly int maxJsonLength;
        private readonly int recursionLimit;

        private readonly JavaScriptSerializer serializer;
        private readonly JavaScriptTypeResolver typeResolver;
        private Dictionary<object, bool> objectCache;
        private int recursionDepth;


        private Dictionary<Type, MethodInfo> serializeGenericDictionaryMethods;

        public JsonSerializer(JavaScriptSerializer serializer)
        {
            if (serializer == null)
                throw new ArgumentNullException("serializer");
            this.serializer = serializer;
            typeResolver = serializer.TypeResolver;
            recursionLimit = serializer.RecursionLimit;
            maxJsonLength = serializer.MaxJsonLength;
        }

        public void Serialize(object obj, StringBuilder output)
        {
            if (output == null)
                throw new ArgumentNullException("output");

            DoSerialize(obj, output);
        }

        public void Serialize(object obj, TextWriter output)
        {
            if (output == null)
                throw new ArgumentNullException("output");

            var sb = new StringBuilder();
            DoSerialize(obj, sb);
            output.Write(sb.ToString());
        }

        private void DoSerialize(object obj, StringBuilder output)
        {
            recursionDepth = 0;
            objectCache = new Dictionary<object, bool>();
            SerializeValue(obj, output);
        }

        private void SerializeValue(object obj, StringBuilder output)
        {
            recursionDepth++;
            SerializeValueImpl(obj, output);
            recursionDepth--;
        }

        private void SerializeValueImpl(object obj, StringBuilder output)
        {
            if (recursionDepth > recursionLimit)
                throw new ArgumentException("Recursion limit has been exceeded while serializing object of type '{0}'",
                    obj != null ? obj.GetType().ToString() : "[null]");

            if (obj == null || DBNull.Value.Equals(obj))
            {
                StringBuilderExtensions.AppendCount(output, maxJsonLength, "null");
                return;
            }

#if !__MonoCS__
            if (obj.GetType().Name == "RuntimeType")
            {
                obj = obj.ToString();
            }
#else
			if (obj.GetType().Name == "MonoType")
			{
				obj = obj.ToString();
			}
#endif
            var valueType = obj.GetType();
            var jsc = serializer.GetConverter(valueType);
            if (jsc != null)
            {
                var result = jsc.Serialize(obj, serializer);

                if (result == null)
                {
                    StringBuilderExtensions.AppendCount(output, maxJsonLength, "null");
                    return;
                }

                if (typeResolver != null)
                {
                    var typeId = typeResolver.ResolveTypeId(valueType);
                    if (!String.IsNullOrEmpty(typeId))
                        result[JavaScriptSerializer.SerializedTypeNameKey] = typeId;
                }

                SerializeValue(result, output);
                return;
            }

            var typeCode = Type.GetTypeCode(valueType);
            switch (typeCode)
            {
                case TypeCode.String:
                    WriteValue(output, (string)obj);
                    return;

                case TypeCode.Char:
                    WriteValue(output, (char)obj);
                    return;

                case TypeCode.Boolean:
                    WriteValue(output, (bool)obj);
                    return;

                case TypeCode.SByte:
                case TypeCode.Int16:
                case TypeCode.UInt16:
                case TypeCode.Int32:
                case TypeCode.Byte:
                case TypeCode.UInt32:
                case TypeCode.Int64:
                case TypeCode.UInt64:
                    if (valueType.IsEnum)
                    {
                        WriteEnumValue(output, obj, typeCode);
                        return;
                    }
                    goto case TypeCode.Decimal;

                case TypeCode.Single:
                    WriteValue(output, (float)obj);
                    return;

                case TypeCode.Double:
                    WriteValue(output, (double)obj);
                    return;

                case TypeCode.Decimal:
                    WriteValue(output, obj as IConvertible);
                    return;

                case TypeCode.DateTime:
                    WriteValue(output, (DateTime)obj);
                    return;
            }

            if (typeof (Uri).IsAssignableFrom(valueType))
            {
                WriteValue(output, (Uri)obj);
                return;
            }

            if (typeof (Guid).IsAssignableFrom(valueType))
            {
                WriteValue(output, (Guid)obj);
                return;
            }

            if (typeof (DynamicDictionaryValue).IsAssignableFrom(valueType))
            {
                var o = (DynamicDictionaryValue)obj;
                SerializeValue(o.Value, output);
                return;
            }

            var convertible = obj as IConvertible;
            if (convertible != null)
            {
                WriteValue(output, convertible);
                return;
            }

            try
            {
                if (objectCache.ContainsKey(obj))
                    throw new InvalidOperationException("Circular reference detected.");
                objectCache.Add(obj, true);

                var closedIDict = GetClosedIDictionaryBase(valueType);
                if (closedIDict != null)
                {
                    if (serializeGenericDictionaryMethods == null)
                        serializeGenericDictionaryMethods = new Dictionary<Type, MethodInfo>();

                    MethodInfo mi;
                    if (!serializeGenericDictionaryMethods.TryGetValue(closedIDict, out mi))
                    {
                        var types = closedIDict.GetGenericArguments();
                        mi = serializeGenericDictionary.MakeGenericMethod(types[0], types[1]);
                        serializeGenericDictionaryMethods.Add(closedIDict, mi);
                    }

                    mi.Invoke(this, new object[] {output, obj});
                    return;
                }

                var dict = obj as IDictionary;
                if (dict != null)
                {
                    SerializeDictionary(output, dict);
                    return;
                }

                var enumerable = obj as IEnumerable;
                if (enumerable != null)
                {
                    SerializeEnumerable(output, enumerable);
                    return;
                }

                SerializeArbitraryObject(output, obj, valueType);
            }
            finally
            {
                objectCache.Remove(obj);
            }
        }

        private Type GetClosedIDictionaryBase(Type t)
        {
            if (t.IsGenericType && typeof (IDictionary<,>).IsAssignableFrom(t.GetGenericTypeDefinition()))
                return t;

            foreach (var iface in t.GetInterfaces())
            {
                if (iface.IsGenericType && typeof (IDictionary<,>).IsAssignableFrom(iface.GetGenericTypeDefinition()))
                    return iface;
            }

            return null;
        }

        private bool ShouldIgnoreMember(MemberInfo mi, out MethodInfo getMethod)
        {
            getMethod = null;
            if (mi == null)
                return true;

            if (mi.IsDefined(typeof (ScriptIgnoreAttribute), true))
                return true;

            var fi = mi as FieldInfo;
            if (fi != null)
                return false;

            var pi = mi as PropertyInfo;
            if (pi == null)
                return true;

            getMethod = pi.GetGetMethod();
            if (getMethod == null || getMethod.GetParameters().Length > 0)
            {
                getMethod = null;
                return true;
            }

            return false;
        }

        private object GetMemberValue(object obj, MemberInfo mi)
        {
            var fi = mi as FieldInfo;

            if (fi != null)
                return fi.GetValue(obj);

            var method = mi as MethodInfo;
            if (method == null)
                throw new InvalidOperationException("Member is not a method (internal error).");

            object ret;

            try
            {
                ret = method.Invoke(obj, null);
            }
            catch (TargetInvocationException niex)
            {
                if (niex.InnerException is NotImplementedException)
                {
                    Console.WriteLine("!!! COMPATIBILITY WARNING. FEATURE NOT IMPLEMENTED. !!!");
                    Console.WriteLine(niex);
                    Console.WriteLine("!!! RETURNING NULL. PLEASE LET MONO DEVELOPERS KNOW ABOUT THIS EXCEPTION. !!!");
                    return null;
                }

                throw;
            }

            return ret;
        }

        private void SerializeArbitraryObject(StringBuilder output, object obj, Type type)
        {
            StringBuilderExtensions.AppendCount(output, maxJsonLength, "{");

            var first = true;
            if (typeResolver != null)
            {
                var typeId = typeResolver.ResolveTypeId(type);
                if (!String.IsNullOrEmpty(typeId))
                {
                    WriteDictionaryEntry(output, first, JavaScriptSerializer.SerializedTypeNameKey, typeId);
                    first = false;
                }
            }

            SerializeMembers<FieldInfo>(type.GetFields(BindingFlags.Public | BindingFlags.Instance), obj, output, ref first);
            SerializeMembers<PropertyInfo>(type.GetProperties(BindingFlags.Public | BindingFlags.Instance), obj, output, ref first);

            StringBuilderExtensions.AppendCount(output, maxJsonLength, "}");
        }

        private void SerializeMembers<T>(T[] members, object obj, StringBuilder output, ref bool first) where T : MemberInfo
        {
            MemberInfo member;
            MethodInfo getMethod;
            string name;

            foreach (var mi in members)
            {
                if (ShouldIgnoreMember(mi as MemberInfo, out getMethod))
                    continue;

                name = mi.Name;
                if (getMethod != null)
                    member = getMethod;
                else
                    member = mi;

                WriteDictionaryEntry(output, first, name, GetMemberValue(obj, member));
                if (first)
                    first = false;
            }
        }

        private void SerializeEnumerable(StringBuilder output, IEnumerable enumerable)
        {
            StringBuilderExtensions.AppendCount(output, maxJsonLength, "[");
            var first = true;
            foreach (var value in enumerable)
            {
                if (!first)
                    StringBuilderExtensions.AppendCount(output, maxJsonLength, ',');
                SerializeValue(value, output);
                if (first)
                    first = false;
            }

            StringBuilderExtensions.AppendCount(output, maxJsonLength, "]");
        }

        private void SerializeDictionary(StringBuilder output, IDictionary dict)
        {
            StringBuilderExtensions.AppendCount(output, maxJsonLength, "{");
            var first = true;

            foreach (DictionaryEntry entry in dict)
            {
                WriteDictionaryEntry(output, first, entry.Key as string, entry.Value);
                if (first)
                    first = false;
            }

            StringBuilderExtensions.AppendCount(output, maxJsonLength, "}");
        }

        private void SerializeGenericDictionary<TKey, TValue>(StringBuilder output, IDictionary<TKey, TValue> dict)
        {
            StringBuilderExtensions.AppendCount(output, maxJsonLength, "{");
            var first = true;

            foreach (var kvp in dict)
            {
                var key = typeof (TKey) == typeof (Guid) ? kvp.Key.ToString() : kvp.Key as string;
                WriteDictionaryEntry(output, first, key, kvp.Value);
                if (first)
                    first = false;
            }

            StringBuilderExtensions.AppendCount(output, maxJsonLength, "}");
        }

        private void WriteDictionaryEntry(StringBuilder output, bool skipComma, string key, object value)
        {
            if (key == null)
                throw new InvalidOperationException("Only dictionaries with keys convertible to string, or guid keys are supported.");

            if (!skipComma)
                StringBuilderExtensions.AppendCount(output, maxJsonLength, ',');

            WriteValue(output, key);
            StringBuilderExtensions.AppendCount(output, maxJsonLength, ':');
            SerializeValue(value, output);
        }

        private void WriteEnumValue(StringBuilder output, object value, TypeCode typeCode)
        {
            switch (typeCode)
            {
                case TypeCode.SByte:
                    StringBuilderExtensions.AppendCount(output, maxJsonLength, (sbyte)value);
                    return;

                case TypeCode.Int16:
                    StringBuilderExtensions.AppendCount(output, maxJsonLength, (short)value);
                    return;

                case TypeCode.UInt16:
                    StringBuilderExtensions.AppendCount(output, maxJsonLength, (ushort)value);
                    return;

                case TypeCode.Int32:
                    StringBuilderExtensions.AppendCount(output, maxJsonLength, (int)value);
                    return;

                case TypeCode.Byte:
                    StringBuilderExtensions.AppendCount(output, maxJsonLength, (byte)value);
                    return;

                case TypeCode.UInt32:
                    StringBuilderExtensions.AppendCount(output, maxJsonLength, (uint)value);
                    return;

                case TypeCode.Int64:
                    StringBuilderExtensions.AppendCount(output, maxJsonLength, (long)value);
                    return;

                case TypeCode.UInt64:
                    StringBuilderExtensions.AppendCount(output, maxJsonLength, (ulong)value);
                    return;

                default:
                    throw new InvalidOperationException(String.Format("Invalid type code for enum: {0}", typeCode));
            }
        }

        private void WriteValue(StringBuilder output, float value)
        {
            StringBuilderExtensions.AppendCount(output, maxJsonLength, value.ToString("r", Json.DefaultNumberFormatInfo));
        }


        private void WriteValue(StringBuilder output, double value)
        {
            StringBuilderExtensions.AppendCount(output, maxJsonLength, value.ToString("r", Json.DefaultNumberFormatInfo));
        }

        private void WriteValue(StringBuilder output, Guid value)
        {
            WriteValue(output, value.ToString());
        }

        private void WriteValue(StringBuilder output, Uri value)
        {
            WriteValue(output, value.GetComponents(UriComponents.AbsoluteUri, UriFormat.UriEscaped));
        }

        private void WriteValue(StringBuilder output, DateTime value)
        {
            var time = value.ToUniversalTime();

            var suffix = "";
            if (value.Kind != DateTimeKind.Utc)
            {
                TimeSpan localTZOffset;
                if (value > time)
                {
                    localTZOffset = value - time;
                    suffix = "+";
                }
                else
                {
                    localTZOffset = time - value;
                    suffix = "-";
                }
                suffix += localTZOffset.ToString("hhmm");
            }

            if (time < MinimumJavaScriptDate)
                time = MinimumJavaScriptDate;

            var ticks = (time.Ticks - InitialJavaScriptDateTicks)/(long)10000;
            StringBuilderExtensions.AppendCount(output, maxJsonLength, "\"\\/Date(" + ticks + suffix + ")\\/\"");
        }

        private void WriteValue(StringBuilder output, IConvertible value)
        {
            StringBuilderExtensions.AppendCount(output, maxJsonLength, value.ToString(CultureInfo.InvariantCulture));
        }

        private void WriteValue(StringBuilder output, bool value)
        {
            StringBuilderExtensions.AppendCount(output, maxJsonLength, value ? "true" : "false");
        }

        private void WriteValue(StringBuilder output, char value)
        {
            if (value == '\0')
            {
                StringBuilderExtensions.AppendCount(output, maxJsonLength, "null");
                return;
            }

            WriteValue(output, value.ToString());
        }

        private void WriteValue(StringBuilder output, string value)
        {
            if (String.IsNullOrEmpty(value))
            {
                StringBuilderExtensions.AppendCount(output, maxJsonLength, "\"\"");
                return;
            }

            StringBuilderExtensions.AppendCount(output, maxJsonLength, "\"");

            char c;
            for (var i = 0; i < value.Length; i++)
            {
                c = value[i];

                switch (c)
                {
                    case '\t':
                        StringBuilderExtensions.AppendCount(output, maxJsonLength, @"\t");
                        break;
                    case '\n':
                        StringBuilderExtensions.AppendCount(output, maxJsonLength, @"\n");
                        break;
                    case '\r':
                        StringBuilderExtensions.AppendCount(output, maxJsonLength, @"\r");
                        break;
                    case '\f':
                        StringBuilderExtensions.AppendCount(output, maxJsonLength, @"\f");
                        break;
                    case '\b':
                        StringBuilderExtensions.AppendCount(output, maxJsonLength, @"\b");
                        break;
                    case '<':
                        StringBuilderExtensions.AppendCount(output, maxJsonLength, @"\u003c");
                        break;
                    case '>':
                        StringBuilderExtensions.AppendCount(output, maxJsonLength, @"\u003e");
                        break;
                    case '"':
                        StringBuilderExtensions.AppendCount(output, maxJsonLength, "\\\"");
                        break;
                    case '\'':
                        StringBuilderExtensions.AppendCount(output, maxJsonLength, @"\u0027");
                        break;
                    case '\\':
                        StringBuilderExtensions.AppendCount(output, maxJsonLength, @"\\");
                        break;
                    default:
                        if (c > '\u001f')
                            StringBuilderExtensions.AppendCount(output, maxJsonLength, c);
                        else
                        {
                            output.Append("\\u00");
                            var intVal = (int)c;
                            StringBuilderExtensions.AppendCount(output, maxJsonLength, (char)('0' + (intVal >> 4)));
                            intVal &= 0xf;
                            StringBuilderExtensions.AppendCount(output, maxJsonLength, (char)(intVal < 10 ? '0' + intVal : 'a' + (intVal - 10)));
                        }
                        break;
                }
            }

            StringBuilderExtensions.AppendCount(output, maxJsonLength, "\"");
        }
    }
}