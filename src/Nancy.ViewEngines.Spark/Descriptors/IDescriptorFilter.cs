namespace Nancy.ViewEngines.Spark.Descriptors
{
    using System.Collections.Generic;

    public interface IDescriptorFilter
    {
        /// <summary>
        /// Called frequently to extract filter-specific parameters from a request viewLocationResult. This call
        /// happens on every request so should be implemented as efficiently as possible.
        /// </summary>
        /// <param name="viewLocationResult">The current request's action viewLocationResult</param>
        /// <param name="extra">Dictionary where additional parameters should be added</param>
        void ExtraParameters(ViewLocationResult viewLocationResult, IDictionary<string, object> extra);

        /// <summary>
        /// The DefaultDescriptorBuilder calls this method for the filter to return a modified enumerable
        /// ordered list of potential template locations. This is called only when the unique combination of action,
        /// master, view, and extra have not been resolved previously.
        /// </summary>
        /// <param name="locations">incoming ordered list of locations</param>
        /// <param name="extra">extra parameters which have been extracted</param>
        /// <returns>either the original list or a new, augmented, enumerable list</returns>
        IEnumerable<string> PotentialLocations(IEnumerable<string> locations, IDictionary<string, object> extra);
    }
}