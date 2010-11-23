namespace Nancy.Tests.Fakes
{
    using System;

    public class FakeNancy : NancyModule
    {
        public static bool WasCreated = false;

        public FakeNancy()
        {
            WasCreated = true;

            Get["/"] = x => {
                throw new NotImplementedException();
            };

            Post["/"] = x => {
                throw new NotImplementedException();
            };
        }
    }
}