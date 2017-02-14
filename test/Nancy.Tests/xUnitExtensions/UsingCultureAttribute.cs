namespace Nancy.Tests.xUnitExtensions
{
    using System;
    using System.Globalization;
    using System.Reflection;
    using System.Threading;
    using Xunit.Sdk;

    /// <summary>
    /// Attribute that is applied to a method to indicate that it should be run using a particular
    /// culture (and UI culture).
    /// Applies the named culture before running the method and restores the original one after.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public sealed class UsingCultureAttribute : BeforeAfterTestAttribute
    {
        private readonly string cultureName;

        private CultureInfo originalCulture;
        private CultureInfo originalUiCulture;

        /// <summary>
        /// Create an instance of <see cref="UsingCultureAttribute"/> that will apply the
        /// name culture during the execution of the decorated method.
        /// </summary>
        /// <param name="cultureName">The name of the culture to use.</param>
        /// <example>
        ///   <code>
        ///     [Fact]
        ///     [UsingCulture("de-DE")]
        ///     public void Should_Behave_In_Some_Way()
        ///     …
        ///   </code>
        /// </example>
        public UsingCultureAttribute(string cultureName)
        {
            this.cultureName = cultureName;
        }

        /// <summary>
        /// This method is called after the test method is executed. It restores the original
        /// current culture and current UI culture in effect before the test method was run.
        /// </summary>
        /// <param name="methodUnderTest">The method under test</param>
        public override void After(MethodInfo methodUnderTest)
        {
#if CORE
            CultureInfo.CurrentCulture = this.originalCulture;
            CultureInfo.CurrentUICulture = this.originalUiCulture;
#else
            Thread.CurrentThread.CurrentCulture = this.originalCulture;
            Thread.CurrentThread.CurrentUICulture = this.originalUiCulture;
#endif
        }

        /// <summary>
        /// This method is called before the test method is executed. It switches the current
        /// culture and current UI culture to the culture chosen when this instance was created.
        /// </summary>
        /// <param name="methodUnderTest">The method under test</param>
        public override void Before(MethodInfo methodUnderTest)
        {
#if CORE
            this.originalCulture = CultureInfo.CurrentCulture;
            this.originalUiCulture = CultureInfo.CurrentUICulture;
            CultureInfo.CurrentCulture = new CultureInfo(this.cultureName);
            CultureInfo.CurrentUICulture = new CultureInfo(this.cultureName);
#else
            this.originalCulture = Thread.CurrentThread.CurrentCulture;
            this.originalUiCulture = Thread.CurrentThread.CurrentUICulture;
            Thread.CurrentThread.CurrentCulture = new CultureInfo(this.cultureName);
            Thread.CurrentThread.CurrentUICulture = new CultureInfo(this.cultureName);
#endif
        }
    }
}
