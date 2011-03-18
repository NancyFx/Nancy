namespace Nancy.MimeType
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Reflection;


    /// <summary>
    /// Defines the default implementation of the <see cref="IMimeTypeRegistry"/>
    /// interface.
    /// </summary>
    public class DefaultMimeTypeRegistry : IMimeTypeRegistry
    {
        private static IDictionary<string, string> map = null;

        /// <summary>
        /// Returns the inferred mime-type for the given extension.  If the 
        /// mime-type can not be inferred then we return "application/octet-stream".
        /// This method reads an embedded resource which is from the mime_types.txt
        /// file.
        /// </summary>
        /// <param name="extension">The extension to infer the mime-type for</param>
        /// <returns>The inferred mime-type for the specified extension</returns>
        public string GetMimeTypeFor(string extension)
        {
            if (extension.EndsWith(".")) { extension = extension.Remove(extension.Length - 1); }
            extension = extension.ToLower();
            return GetMimeTypeForImpl(extension);
        }

        private string GetMimeTypeForImpl(string extension)
        {
            if (map == null) lock (GetType()) if (map == null)
                InitialiseMap();

            return map.ContainsKey(extension) ? 
                map[extension] :
                "application/octet-stream";
        }

        private static void InitialiseMap()
        {
            map = new Dictionary<string, string>();
            using (StreamReader r = new StreamReader(Assembly.GetExecutingAssembly().GetManifestResourceStream("Nancy.MimeType.mime_types.txt")))
                Array.ForEach(r.ReadToEnd().Split('\n'), ParseLine);
        }

        private static void ParseLine(string line)
        {
            int idx = line.IndexOf(": ");            
            if (idx <= 0) return; // Invalid line?
            string key = line.Substring(0, idx).Trim().ToLower();
            if (map.ContainsKey(key)) return; // Duplicate declaration, ignore

            string val = line.Substring(idx + 2).Trim();
            map.Add(key, val);
        }
    }
}