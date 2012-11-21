namespace Nancy.ErrorHandling
{
    using System;

    /// <summary>
    /// Provides informative responses for particular HTTP status codes
    /// </summary>
    [Obsolete("This interface has been superseded by the IStatusCodeHandler interface, and will be removed in a subsequent release.")]
    public interface IErrorHandler : IStatusCodeHandler
    {
    }
}