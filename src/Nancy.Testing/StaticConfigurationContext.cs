namespace Nancy.Testing
{
    using System;

    /// <summary>
    /// Helper class for running tests with <see cref="StaticConfiguration"/> setup in a certain way. This
    /// class was designed to be used with a using-statement. Upon disposable the previous static configuration
    /// values will be reset, leaving it in the state is was before the test.
    /// </summary>
    public class StaticConfigurationContext : IDisposable
    {
        private readonly StaticConfigurationValues existingConfiguration = new StaticConfigurationValues();

        /// <summary>
        /// Initializes a new instance of the <see cref="StaticConfigurationContext"/> class.
        /// </summary>
        /// <param name="closure">The configuration context.</param>
        public StaticConfigurationContext(Action<StaticConfigurationValues> closure)
        {
            this.existingConfiguration.CaseSensitive = StaticConfiguration.CaseSensitive;
            this.existingConfiguration.RequestQueryFormMultipartLimit = StaticConfiguration.RequestQueryFormMultipartLimit;

            var temporaryConfiguration =
                new StaticConfigurationValues();

            closure.Invoke(temporaryConfiguration);

            AssignStaticConfigurationValues(temporaryConfiguration);
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            AssignStaticConfigurationValues(this.existingConfiguration);
        }

        private static void AssignStaticConfigurationValues(StaticConfigurationValues values)
        {
            StaticConfiguration.CaseSensitive = values.CaseSensitive;
            StaticConfiguration.RequestQueryFormMultipartLimit = values.RequestQueryFormMultipartLimit;
        }

        /// <summary>
        /// Helper class used to persist state of <see cref="StaticConfiguration"/> members.
        /// </summary>
        public class StaticConfigurationValues
        {
            public bool CaseSensitive { get; set; }

            public int RequestQueryFormMultipartLimit { get; set; }
        }
    }
}
