namespace Nancy.ViewEngines
{
    using System;
    using System.IO;

    public class FileViewLocationResult : IViewLocationResult
    {
        private readonly FileInfo fileInfo;

        public FileViewLocationResult(FileInfo fileInfo)
        {
            this.fileInfo = fileInfo;
            this.Contents = fileInfo.OpenText();
        }

        #region IViewLocationResult Members

        public string Location
        {
            get { return fileInfo.FullName; }
        }

        public DateTime LastModified
        {
            get { return fileInfo.LastWriteTime; }
        }

        public TextReader Contents { get; private set; }

        #endregion
    }
}