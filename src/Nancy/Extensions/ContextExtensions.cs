namespace Nancy.Extensions
{
    public static class ContextExtensions
    {
        /// <summary>
        /// An extension method making it easy to check if the reqeuest was done using ajax
        /// </summary>
        /// <param name="context">The current nancy context</param>
        /// <returns>True if the request was done using ajax</returns>
        public static bool IsAjaxRequest(this NancyContext context)
        {
            return context.Request != null && context.Request.IsAjaxRequest();
        }
    }
}