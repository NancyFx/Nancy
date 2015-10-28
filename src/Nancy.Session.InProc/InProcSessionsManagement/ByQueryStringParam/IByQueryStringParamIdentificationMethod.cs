namespace Nancy.Session.InProc.InProcSessionsManagement.ByQueryStringParam
{
    /// <summary>
    /// Identification method for in-process memory based sessions.
    /// </summary>
    public interface IByQueryStringParamIdentificationMethod : IInProcSessionIdentificationMethod
    {
        /// <summary>
        /// Gets or sets the querystring parameter name in which the session id is stored.
        /// </summary>
        string ParameterName
        {
            get;
            set;
        }
    }
}