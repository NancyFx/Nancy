namespace Nancy.Session.InProc.InProcSessionsManagement
{
    using System;
    using Nancy.Cryptography;

    internal class HmacValidator : IHmacValidator
    {
        private readonly IHmacProvider hmacProvider;

        public HmacValidator(IHmacProvider hmacProvider)
        {
            if (hmacProvider == null)
            {
                throw new ArgumentNullException("hmacProvider");
            }
            this.hmacProvider = hmacProvider;
        }

        public bool IsValidHmac(SessionIdentificationData sessionIdentificationData)
        {
            if (sessionIdentificationData == null)
            {
                throw new ArgumentNullException("sessionIdentificationData");
            }
            if (sessionIdentificationData.Hmac == null)
            {
                return false;
            }

            var incomingBytes = sessionIdentificationData.Hmac;
            var expectedHmac = this.hmacProvider.GenerateHmac(sessionIdentificationData.SessionId);
            return HmacComparer.Compare(expectedHmac, incomingBytes, this.hmacProvider.HmacLength);
        }
    }
}