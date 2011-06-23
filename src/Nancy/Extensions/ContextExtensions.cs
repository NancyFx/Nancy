namespace Nancy.Extensions
{
    public static class ContextExtensions
    {
        public static bool IsAjaxRequest(this NancyContext context)
        {
            return context.Request != null && context.Request.IsAjaxRequest();
        }
    }
}