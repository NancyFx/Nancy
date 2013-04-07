namespace Nancy.Conventions
{
    /// <summary>
    /// Nancy static directory convention helper
    /// </summary>
    public class StaticDirectoryContent
    {
        NancyConventions conventions;

        /// <summary>
        /// Creates a new instance of StaticDirectoryContent
        /// </summary>
        /// <param name="conventions">NancyConventions, to wich static directories get added</param>
        public StaticDirectoryContent(NancyConventions conventions)
        {
            this.conventions = conventions;
        }

        /// <summary>
        /// Adds a new static directory to the nancy conventions
        /// </summary>
        /// <param name="requestFile">The route of the file</param>
        public string this[string requestDirectory]
        {
            set
            {
                conventions.StaticContentsConventions.Add(StaticContentConventionBuilder.AddDirectory(requestDirectory, value));
            }
        }
    }
}
