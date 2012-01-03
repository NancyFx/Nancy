namespace Nancy.ViewEngines
{
    using System;

    public class ViewNotFoundException : Exception
    {
        public string ViewName { get; private set; }

        public string[] AvailableViewEngineExtensions { get; private set; }

        public ViewNotFoundException(string viewName, string[] availableViewEngineExtensions)
        {
            this.ViewName = viewName;
            this.AvailableViewEngineExtensions = availableViewEngineExtensions;
        }
    }
}