namespace Nancy
{
	public class HeadResponse : Response
	{
		public HeadResponse(Response response)
		{
		    this.Contents = GetStringContents(string.Empty);
			this.ContentType = response.ContentType;
		    this.Headers = response.Headers;
			this.StatusCode = response.StatusCode;
		}
	}
}