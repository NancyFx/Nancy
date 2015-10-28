namespace Nancy.Session.InProc.InProcSessionsManagement.ByQueryStringParam
{
    using System;
    using Nancy.Cryptography;

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

        public SessionIdentificationData ProvideDataFromQuery(Request request, string parameterName)
        {
            if (request == null)
            {
                throw new ArgumentNullException("request");
            }
            if (string.IsNullOrWhiteSpace(parameterName))
            {
                throw new ArgumentNullException("parameterName");
            }

            var querystringDictionary = request.Query.ToDictionary();
            if (querystringDictionary == null || !querystringDictionary.ContainsKey(parameterName))
            {
                return null;
            }

            string parameterValue = querystringDictionary[parameterName];
            var hmacLength = Base64Helpers.GetBase64Length(this.hmacProvider.HmacLength);

            if (parameterValue.Length < hmacLength)
            {
                // Definitely invalid
                return null;
            }

            var hmacString = parameterValue.Substring(0, hmacLength);
            var encryptedSessionId = parameterValue.Substring(hmacLength);

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