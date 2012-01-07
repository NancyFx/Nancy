using System;
using Nancy.ErrorHandling;

namespace Nancy.Testing
{
    public class PassThroughErrorHandler : IErrorHandler
    {
        public bool HandlesStatusCode(HttpStatusCode statusCode)
        {
            return statusCode == HttpStatusCode.InternalServerError;
        }

        public void Handle(HttpStatusCode statusCode, NancyContext context)
        {
            throw new Exception("ConfigurableBootstrapper Exception", context.Items[NancyEngine.ERROR_EXCEPTION] as Exception);
        }
    }
}