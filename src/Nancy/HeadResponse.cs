using System;

namespace Nancy
{
	public class HeadResponse : Response
	{
		public HeadResponse(Response response)
		{
			Contents = stream => { }; // head should not return any content
			ContentType = response.ContentType;
			StatusCode = response.StatusCode;
		}
	}
}