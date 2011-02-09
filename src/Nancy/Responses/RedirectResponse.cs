namespace Nancy.Responses
{
    using System.Net;

    public class RedirectResponse : Response
	{
		public RedirectResponse (string location) 
		{
			this.Headers.Add("Location",location);
			this.Contents = GetStringContents(string.Empty);
            this.ContentType = "text/html";
            this.StatusCode = HttpStatusCode.SeeOther;
		}
	}
}