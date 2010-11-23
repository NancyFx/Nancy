namespace Nancy.Tests.Fakes
{
    using FakeItEasy;

    public class FakeRoute : Route
    {
        public bool ActionWasInvoked;

        public FakeRoute()
        {
            this.Action = x => {
                this.ActionWasInvoked = true;
                return new Response();
            };
        }
    }
}