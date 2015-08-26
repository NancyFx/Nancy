namespace Nancy.Tests.Fakes
{
    using System;
    using Nancy.Conventions;

    /// <summary>
    /// A do-nothing IConvention that demonstrates a bug in resolving conventions
    /// </summary>
    public class FakeConventionWithDependencies : IConvention {
        private Person dependantUpon;
        public FakeConventionWithDependencies(Person dependantUpon) {
            this.dependantUpon = dependantUpon;
        }

        public void Initialise(NancyConventions conventions) {
        }

        public Tuple<bool, string> Validate(NancyConventions conventions) {
            return Tuple.Create(true, (string)null);
        }
    }
}
