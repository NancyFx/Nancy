namespace Nancy.HttpParser
{
    using System;
    using System.Collections.Generic;
    using System.Collections.Specialized;
    using System.IO;
    using System.Text;

    internal static class HeaderParser
    {
        private const int CR = '\r';
        private const int LF = '\n';

        public static IDictionary<string, IEnumerable<string>> Parse(Stream stream)
        {
            var headers = new NameValueCollection();
            var bytes = ReadHeaderBytes(stream);

            var text = Encoding.Default.GetString(bytes);
            var lines = text.Split(new[] { "\r\n" }, StringSplitOptions.None);
            foreach (var line in lines)
            {
                if (line == "")
                {
                    break;
                }

                var colonIndex = line.IndexOf(':');
                headers.Add(line.Substring(0, colonIndex), line.Substring(colonIndex + 1).TrimStart());
            }

            var dict = new Dictionary<string, IEnumerable<string>>(headers.Keys.Count);
            foreach (var key in headers.AllKeys)
            {
                dict.Add(key, headers.GetValues(key));
            }

            return dict;
        }

        private static byte[] ReadHeaderBytes(Stream stream)
        {
            var bytes = new List<byte>();
            bool mightBeEnd = false;

            while (true)
            {
                int b = stream.ReadByte();
                if (b == CR)
                {
                    b = stream.ReadByte();
                    if (b != LF)
                    {
                        bytes.Add(CR);
                        bytes.Add((byte)b);
                        continue;
                    }

                    if (mightBeEnd)
                    {
                        break;
                    }

                    mightBeEnd = true;
                    bytes.Add(CR);
                    bytes.Add(LF);
                    continue;
                }

                mightBeEnd = false;
                bytes.Add((byte)b);
            }

            return bytes.ToArray();
        }
    }
}