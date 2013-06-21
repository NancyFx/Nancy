// ReSharper disable CheckNamespace
namespace Nancy
// ReSharper restore CheckNamespace
{
    public static class NancyModuleExtensions
    {
        public static void RequiresClaimsAuthentication(this INancyModule module)
        {
            module.Before.AddItemToEndOfPipeline(
                context => 
                    context.GetClaimsPrincipal() == null ?
                    new Response {StatusCode = HttpStatusCode.Unauthorized } :
                    null);
        }
    }
}