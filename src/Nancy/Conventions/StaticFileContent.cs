namespace Nancy.Conventions
{
    /// <summary>
    /// Nancy static file convention helper
    /// </summary>
    public class StaticFileContent
    {
        private readonly NancyConventions conventions;

        /// <summary>
        /// Creates a new instance of StaticFileContent
        /// </summary>
        /// <param name="conventions">NancyConventions, to which static files get added</param>
        public StaticFileContent(NancyConventions conventions)
        {
            this.conventions = conventions;
        }

        /// <summary>
        /// Adds a new static file to the nancy conventions
        /// </summary>
        /// <param name="requestFile">The route of the file</param>
        public string this[string requestFile]
        {
            set
            {
                this.conventions.StaticContentsConventions.Add(StaticContentConventionBuilder.AddFile(requestFile, value));
            }
        }
    }
}
