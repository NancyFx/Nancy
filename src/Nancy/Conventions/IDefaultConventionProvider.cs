namespace Nancy.Conventions
{
    /// <summary>
    /// Provides a default set of conventions
    /// </summary>
    public interface IDefaultConventionProvider
    {
        /// <summary>
        /// Initialise any conventions this class "owns"
        /// </summary>
        /// <param name="conventions">Convention object instance</param>
        void Initialise(NancyConventions conventions);
    }
}