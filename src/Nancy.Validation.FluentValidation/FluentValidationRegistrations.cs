namespace Nancy.Validation.FluentValidation
{
    using global::FluentValidation;

    using Nancy.Bootstrapper;

    /// <summary>
    /// Application registrations for Fluent Validation types.
    /// </summary>
    public class FluentValidationRegistrations : Registrations
    {
        /// <summary>
        /// Creates a new instance of the <see cref="FluentValidationRegistrations"/> class, that performs
        /// the default registrations of the Fluent Validation types.
        /// </summary>
        public FluentValidationRegistrations()
        {
            this.Register<IFluentAdapterFactory>(typeof(DefaultFluentAdapterFactory));
            this.RegisterAll<IFluentAdapter>();
            this.RegisterAll<IValidator>();
        }
    }
}