namespace Nancy.Routing
{
    /// <summary>
    /// Information about a segment parameter.
    /// </summary>
    public class ParameterSegmentInformation
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ParameterSegmentInformation"/> class.
        /// </summary>
        /// <param name="name">The name of the parameter</param>
        /// <param name="defaultValue">The default value, if any, of the parameter.</param>
        /// <param name="isOptional"><see langword="true"/> if the parameter is optional, otherwise <see langword="false" />.</param>
        public ParameterSegmentInformation(string name, string defaultValue, bool isOptional)
        {
            this.Name = name;
            this.DefaultValue = defaultValue;
            this.IsOptional = isOptional;
        }

        /// <summary>
        /// Gets the default value for the parameter.
        /// </summary>
        public string DefaultValue { get; private set; }

        /// <summary>
        /// Gets the full name of the segment.
        /// </summary>
        /// <remarks>Returns a string in one of the formats: {name}, {name?}, {name?defaultValue} depending on the kind of parameter.</remarks>
        public string FullSegmentName
        {
            get
            {
                return (this.IsOptional) ?
                    string.Concat(this.Name, "?", this.DefaultValue) :
                    this.Name;
            }
        }

        /// <summary>
        /// Gets whether or not the parameter is optional.
        /// </summary>
        /// <value><see langword="true"/> if the parameter is optional, otherwise <see langword="false" />.</value>
        public bool IsOptional { get; private set; }

        /// <summary>
        /// Gets the name of the parameter.
        /// </summary>
        public string Name { get; private set; }
    }
}