namespace Nancy.ViewEngines
{
    using System;

    public class ViewNotFoundException : Exception
    {
        public string ViewName { get; private set; }

        public string[] AvailableViewEngineExtensions { get; private set; }
        public string[] InspectedLocations { get; private set; }

        public ViewNotFoundException(string viewName, string[] availableViewEngineExtensions, string[] inspectedLocations)
        {
            this.ViewName = viewName;
            this.AvailableViewEngineExtensions = availableViewEngineExtensions;
            this.InspectedLocations = inspectedLocations;
        }

        public override string Message
        {
            get
            {
                return String.Format(
                    "Unable to locate view '{0}'. Currently available view engine extensions: {1}. Locations inspected: {2}", 
                    this.ViewName,
                    string.Join(",", this.AvailableViewEngineExtensions),
                    string.Join(",", this.InspectedLocations));
            }
        }
    }
}