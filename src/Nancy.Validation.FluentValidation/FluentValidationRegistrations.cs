namespace Nancy.Validation.FluentValidation
{
    using Bootstrapper;

    using global::FluentValidation;

    /// <summary>
    /// Application registrations for Fluent Validation types.
    /// </summary>
    public class FluentValidationRegistrations : Bootstrapper.Registrations
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