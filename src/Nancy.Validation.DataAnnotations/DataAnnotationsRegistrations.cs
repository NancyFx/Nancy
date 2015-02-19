namespace Nancy.Validation.DataAnnotations
{
    using Nancy.Bootstrapper;

    /// <summary>
    /// Application registrations for Data Annotations validation types.
    /// </summary>
    public class DataAnnotationsRegistrations : Registrations
    {
        /// <summary>
        /// Creates a new instance of the <see cref="DataAnnotationsRegistrations"/> class, that performs
        /// the default registrations of the Data Annotations types.
        /// </summary>
        public DataAnnotationsRegistrations()
        {
            this.RegisterAll<IDataAnnotationsValidatorAdapter>();
            this.RegisterWithDefault<IPropertyValidatorFactory>(typeof(DefaultPropertyValidatorFactory));
            this.RegisterWithDefault<IValidatableObjectAdapter>(typeof(DefaultValidatableObjectAdapter));
        }
    }
}