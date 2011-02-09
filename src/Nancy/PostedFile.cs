using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;

namespace Nancy
{
    public class PostedFileCollection : IEnumerable<PostedFile>
    {
        readonly Dictionary<string, PostedFile> _files;
        public PostedFileCollection() { }
        public PostedFileCollection(IEnumerable<PostedFile> files) { _files = files.ToDictionary(x => x.FileName); }

        public virtual PostedFile this[string name] { get { return _files[name]; }}
        public virtual PostedFile this[int index] { get { return _files.ElementAt(index).Value; }}
        public IEnumerator<PostedFile> GetEnumerator() { return _files.Values.GetEnumerator(); }
        IEnumerator IEnumerable.GetEnumerator() { return GetEnumerator(); }
    }

    public class PostedFile
    {
        public int ContentLength { get; set; }
        public string ContentType { get; set; }
        public string FileName { get; set; }
        public Stream InputStream { get; set; }
    }
}