namespace Nancy.Session.InProc.InProcSessionsManagement.BySessionIdCookie
{
    using System;
    using Nancy.Cryptography;
    using Nancy.Helpers;

    internal class SessionIdentificationDataProvider : ISessionIdentificationDataProvider
    {
        private readonly IHmacProvider hmacProvider;

        public SessionIdentificationDataProvider(IHmacProvider hmacProvider)
        {
            if (hmacProvider == null)
            {
                throw new ArgumentNullException("hmacProvider");
            }
            this.hmacProvider = hmacProvider;
        }

        public SessionIdentificationData ProvideDataFromCookie(Request request, string cookieName)
        {
            if (request == null)
            {
                throw new ArgumentNullException("request");
            }
            if (string.IsNullOrWhiteSpace(cookieName))
            {
                throw new ArgumentNullException("cookieName");
            }

            string cookieValue = null;
            if (!request.Cookies.TryGetValue(cookieName, out cookieValue))
            {
                return null;
            }

            var decodedCookieValue = HttpUtility.UrlDecode(cookieValue);
            var hmacLength = Base64Helpers.GetBase64Length(this.hmacProvider.HmacLength);

            if (decodedCookieValue.Length < hmacLength)
            {
                // Definitely invalid
                return null;
            }

            var hmacString = decodedCookieValue.Substring(0, hmacLength);
            var encryptedSessionId = decodedCookieValue.Substring(hmacLength);

            byte[] hmacBytes;
            try
            {
                hmacBytes = Convert.FromBase64String(hmacString);
            }
            catch (FormatException)
            {
                // Invalid HMAC
                return null;
            }

            return new SessionIdentificationData
            {
                SessionId = encryptedSessionId,
                Hmac = hmacBytes
            };
        }
    }
}