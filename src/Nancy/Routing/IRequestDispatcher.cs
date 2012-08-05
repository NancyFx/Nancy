namespace Nancy.Routing
{
    /// <summary>
    /// Functionality for processing an incoming request.
    /// </summary>
    public interface IRequestDispatcher
    {
        /// <summary>
        /// Dispatches a requests.
        /// </summary>
        /// <param name="context">The <see cref="NancyContext"/> for the current request.</param>
        void Dispatch(NancyContext context);
    }
}