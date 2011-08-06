namespace Nancy.Security
{
    using System;
    using Bootstrapper;

    public static class CsrfProtection
    {
        public static void RequiresCsrfProtection(this NancyModule module)
        {
            
        }

        public static void Enable(IApplicationPipelines pipelines)
        {
            pipelines.BeforeRequest.AddItemToStartOfPipeline(getCsrfValidationHook());
        }

        private static Func<NancyContext, Response> getCsrfValidationHook()
        {
            throw new NotImplementedException();
        }
    }
}