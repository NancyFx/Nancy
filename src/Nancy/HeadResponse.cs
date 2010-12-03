using System;

namespace Nancy
{
	public class HeadResponse : Response
	{
		public HeadResponse(Response response)
		{
			Contents = String.Empty; // head should not return any content
			ContentType = response.ContentType;
			StatusCode = response.StatusCode;
		}
	}
}