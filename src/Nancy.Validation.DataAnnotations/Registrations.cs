namespace Nancy.Validation.DataAnnotations
{
    using Bootstrapper;

    /// <summary>
    /// Application registrations for Data Annotations validation types.
    /// </summary>
    public class Registrations : ApplicationRegistrations
    {
        /// <summary>
        /// Creates a new instance of the <see cref="Registrations"/> class, that performs
        /// the default registrations of the Data Annotations types.
        /// </summary>
        public Registrations()
        {
            this.Register<IModelValidator>(typeof(DataAnnotationsValidator));
            this.Register<IModelValidatorFactory>(typeof(DataAnnotationsValidatorFactory));
            this.RegisterAll<IDataAnnotationsValidatorAdapter>();
            this.RegisterWithDefault<IPropertyValidatorFactory>(typeof(DefaultPropertyValidatorFactory));
            this.RegisterWithDefault<IValidatableObjectAdapter>(typeof(DefaultValidatableObjectAdapter));
        }
    }
}