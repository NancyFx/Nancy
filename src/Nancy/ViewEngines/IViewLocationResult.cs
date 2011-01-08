namespace Nancy.ViewEngines
{
    using System;
    using System.IO;

    public interface IViewLocationResult
    {
        string Location { get; }
        DateTime LastModified { get; }
        TextReader Contents { get; }
    }
}