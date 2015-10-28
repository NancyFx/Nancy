namespace Nancy.Conventions
{
    /// <summary>
    /// Nancy static directory convention helper
    /// </summary>
    public class StaticDirectoryContent
    {
        private readonly NancyConventions conventions;

        /// <summary>
        /// Creates a new instance of StaticDirectoryContent
        /// </summary>
        /// <param name="conventions">NancyConventions, to which static directories get added</param>
        public StaticDirectoryContent(NancyConventions conventions)
        {
            this.conventions = conventions;
        }

        /// <summary>
        /// Adds a new static directory to the nancy conventions
        /// </summary>
        /// <param name="requestDirectory">The route of the file</param>
        /// <param name="allowedExtensions">A list of extensions that is valid for the conventions. If not supplied, all extensions are valid</param>
        public string this[string requestDirectory, params string[] allowedExtensions]
        {
            set
            {
                this.conventions.StaticContentsConventions.Add(StaticContentConventionBuilder.AddDirectory(requestDirectory, value, allowedExtensions));
            }
        }
    }
}
