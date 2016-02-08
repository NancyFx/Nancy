namespace Nancy
{
    using System.Collections.Generic;

    /// <summary>
    /// Culture configuration
    /// </summary>
    public class CultureConfiguration
    {
        /// <summary>
        /// A default instance of the <see cref="CultureConfiguration"/> class
        /// </summary>
        public static readonly CultureConfiguration Default = new CultureConfiguration(cultureNames: new[] { "en-us" });

        /// <summary>
        /// Initializes a new instance of the <see cref="CultureConfiguration"/> class
        /// </summary>
        /// <param name="cultureNames"></param>
        public CultureConfiguration(IEnumerable<string> cultureNames)
        {
            this.CultureNames = cultureNames;
        }

        /// <summary>
        /// A set of allowed cultures
        /// </summary>
        public IEnumerable<string> CultureNames { get; private set; }
    }
}
