namespace Nancy.Extensions
{
	using Nancy;

	public static class ResponseExtensions
	{
	   public static Response AsRedirect(this IResponseFormatter response, string location)
	   {
	      return new RedirectResponse(location);
	   }
	} 
}

