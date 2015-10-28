namespace Nancy.Session.InProc.Tests.InProcSessionsManagement
{
    using Nancy.Session.InProc.InProcSessionsManagement;
    using Xunit;

    public class SessionIdentificationDataFixture
    {
        private readonly string hmacString;
        private readonly SessionIdentificationData sessionIdentificationData;

        public SessionIdentificationDataFixture()
        {
            this.sessionIdentificationData = new SessionIdentificationData
            {
                SessionId = "TheSessionId",
                Hmac = new byte[] {211, 81, 204, 0, 47, 124}
            };
            this.hmacString = "01HMAC98";
        }

        [Fact]
        public void ToString_returns_expected_string_representation()
        {
            var actual = this.sessionIdentificationData.ToString();
            var expected = string.Format("{0}{1}", this.hmacString, this.sessionIdentificationData.SessionId);
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void ToString_returns_value_without_hmac_if_there_is_no_hmac()
        {
            this.sessionIdentificationData.Hmac = null;
            var actual = this.sessionIdentificationData.ToString();
            Assert.Equal(this.sessionIdentificationData.SessionId, actual);
        }
    }
}