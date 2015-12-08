namespace Nancy.Validation.FluentValidation.Tests
{
    using global::FluentValidation.Internal;
    using global::FluentValidation.Validators;

    public class EmailAdapterFixture
    {
        public EmailAdapterFixture()
        {
            //var member =
            //    typeof (ClassUnderTest).GetProperty("Email");

            //var rule = 
            //    new PropertyRule(member, )
        }

        private class ClassUnderTest
        {
            public string Email { get; set; }
        }
    }
}