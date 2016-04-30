namespace Nancy.Validation.DataAnnotations.Tests
{
    using FakeItEasy;

    public abstract class BaseValidatorAdapterTestFixture<T> where T : IDataAnnotationsValidatorAdapter, new()
    {
        protected readonly IModelValidatorFactory factory;

        protected BaseValidatorAdapterTestFixture()
        {
            var adapterFactory = new DefaultPropertyValidatorFactory(new IDataAnnotationsValidatorAdapter[]
            {
                new T()
            });

            var validator = A.Fake<IValidatableObjectAdapter>();

            this.factory = new DataAnnotationsValidatorFactory(adapterFactory, validator);
        }
    }
}
