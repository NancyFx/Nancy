namespace Nancy.Tests.Fakes
{
    using System;
    using System.IO;
    using ViewEngines;

    public class FakeViewLocationResult : IViewLocationResult
    {
        public FakeViewLocationResult(string text, DateTime lastModified)
        {
            Contents = new StringReader(text);
            LastModified = lastModified;
        }

        public FakeViewLocationResult(string text) : this(text, DateTime.Now)
        {
        }

        public void ChangeContents(string text, DateTime lastModified)
        {
            Contents = new StringReader(text);
            LastModified = lastModified;
        }

        #region IViewLocationResult Members

        public string Location
        {
            get { return "in/memory"; }
        }

        public DateTime LastModified { get; private set; }

        public TextReader Contents { get; private set; }

        #endregion
    }
}