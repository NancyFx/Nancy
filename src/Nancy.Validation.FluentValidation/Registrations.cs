namespace Nancy.Validation.FluentValidation
{
    using Bootstrapper;

    using global::FluentValidation;

    /// <summary>
    /// Application registrations for Fluent Validation types.
    /// </summary>
    public class Registrations : ApplicationRegistrations
    {
        /// <summary>
        /// Creates a new instance of the <see cref="Registrations"/> class, that performs
        /// the default registrations of the Fluent Validation types.
        /// </summary>
        public Registrations()
        {
            this.Register<IModelValidator>(typeof(FluentValidationValidator));
            this.Register<IModelValidatorFactory>(typeof(FluentValidationValidatorFactory));
            this.Register<IFluentAdapterFactory>(typeof(DefaultFluentAdapterFactory));

            this.RegisterAll<IFluentAdapter>();
            this.RegisterAll<IValidator>();
        }
    }
}