namespace Nancy.ViewEngines.Razor
{
    using System;
    using System.IO;
    using Microsoft.Extensions.FileProviders;

    public class ViewLocationFileInfo : IFileInfo
    {
        private readonly ViewLocationResult viewLocation;

        public ViewLocationFileInfo(ViewLocationResult viewLocation)
        {
            this.viewLocation = viewLocation;
        }

        public bool Exists
        {
            get { return this.viewLocation != null; }
        }

        public long Length
        {
            get
            {
                if (this.viewLocation != null
                    && this.viewLocation.Contents != null)
                {
                    var reader = this.viewLocation.Contents();
                    if (reader != null)
                    {
                        return reader.ReadToEnd().Length;
                    }
                }
                return 0;
            }
        }

        public string PhysicalPath
        {
            get { return this.viewLocation != null ? (this.viewLocation.Location ?? null) : null; }
        }

        public string Name
        {
            get { return this.viewLocation != null ? (this.viewLocation.Name ?? null) : null; }
        }

        public DateTimeOffset LastModified
        {
            get { return DateTimeOffset.Now; }
        }

        public bool IsDirectory
        {
            get { return false; }
        }

        public Stream CreateReadStream()
        {
            TextReader reader = null;
            if (this.viewLocation != null)
            {
                reader = this.viewLocation.Contents != null ? this.viewLocation.Contents() : null;
            }
            if (reader == null)
            {
                reader = new StringReader(string.Empty);
            }
            var streamReader = reader as StreamReader;
            if (streamReader != null && streamReader.BaseStream != null)
            {
                return streamReader.BaseStream;
            }
            return new TextReaderStream(reader);
        }
    }
}