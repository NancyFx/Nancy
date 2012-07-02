namespace Nancy.Routing.CORS
{
    using System;

    public class CORSRequestException : Exception
    {
        public CORSRequestException(string message) : base (message)
        {
        }
    }
}