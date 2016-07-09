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
        /// <param name="typeCatalog">An <see cref="ITypeCatalog"/> instance.</param>
        public DataAnnotationsRegistrations(ITypeCatalog typeCatalog) : base(typeCatalog)
        {
            this.RegisterAll<IDataAnnotationsValidatorAdapter>();
            this.RegisterWithDefault<IPropertyValidatorFactory>(typeof(DefaultPropertyValidatorFactory));
            this.RegisterWithDefault<IValidatableObjectAdapter>(typeof(DefaultValidatableObjectAdapter));
        }
    }
}
