namespace Nancy.Owin
{
    using System.Collections.Generic;
    using System.Linq;

    using Nancy.Bootstrapper;

    public class NancyOptions
    {
        private INancyBootstrapper bootstrapper;
        private IEnumerable<int> passThroughStatusCodes;

        public INancyBootstrapper Bootstrapper
        {
            get { return this.bootstrapper ?? NancyBootstrapperLocator.Bootstrapper; }
            set { this.bootstrapper = value; }
        }

        public IEnumerable<int> PassThroughStatusCodes
        {
            get { return this.passThroughStatusCodes ?? Enumerable.Empty<int>(); }
            set { this.passThroughStatusCodes = value; }
        }
    }
}