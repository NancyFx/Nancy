using System;
using Nancy.ErrorHandling;

namespace Nancy.Testing
{
    public class PassThroughErrorHandler : IErrorHandler
    {
        public bool HandlesStatusCode(HttpStatusCode statusCode, NancyContext context)
        {
            var exception = context.Items[NancyEngine.ERROR_EXCEPTION] as Exception;

            return statusCode == HttpStatusCode.InternalServerError && exception != null;
        }

        public void Handle(HttpStatusCode statusCode, NancyContext context)
        {
            throw new Exception("ConfigurableBootstrapper Exception", context.Items[NancyEngine.ERROR_EXCEPTION] as Exception);
        }
    }
}