namespace Nancy.Session
{
    /// <summary>
    /// Allows setting of the formatter for session object storage
    /// </summary>
    public interface IFormatterSelector : IHideObjectMembers
    {
        /// <summary>
        /// Using the specified formatter
        /// </summary>
        /// <param name="newFormatter">Formatter to use</param>
        void WithFormatter(ISessionObjectFormatter newFormatter);
    }
}