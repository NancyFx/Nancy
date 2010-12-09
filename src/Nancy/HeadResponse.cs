namespace Nancy
{
	public class HeadResponse : Response
	{
		public HeadResponse(Response response)
		{
		    Contents = GetStringContents(string.Empty);
			ContentType = response.ContentType;
			StatusCode = response.StatusCode;
		}
	}
}