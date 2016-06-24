namespace Nancy.Diagnostics
{
    /// <summary>
    /// 
    /// </summary>
    /// <seealso cref="Nancy.Diagnostics.IDiagnosticsProvider" />
    public class TestingDiagnosticProvider : IDiagnosticsProvider
    {
        private readonly object diagObject;

        /// <summary>
        /// Initializes a new instance of the <see cref="TestingDiagnosticProvider"/> class.
        /// </summary>
        public TestingDiagnosticProvider()
        {
            this.diagObject = new DiagObject();
        }

        /// <summary>
        /// Gets the name of the provider.
        /// </summary>
        /// <value>A <see cref="string"/> containing the name of the provider.</value>
        public string Name
        {
            get { return "Testing Diagnostic Provider"; }
        }

        /// <summary>
        /// Gets the description of the provider.
        /// </summary>
        /// <value>A <see cref="string"/> containing the description of the provider.</value>
        public string Description
        {
            get { return "Some testing methods that can be called to.. erm.. test things."; }
        }

        /// <summary>
        /// Gets the object that contains the interactive diagnostics methods.
        /// </summary>
        /// <value>An instance of the interactive diagnostics object.</value>
        public object DiagnosticObject
        {
            get { return this.diagObject; }
        }

        /// <summary>
        /// 
        /// </summary>
        public class DiagObject
        {
            /// <summary>
            /// Empty return value
            /// </summary>
            public void NoReturnValue()
            {

            }

            /// <summary>
            /// String the return value.
            /// </summary>
            /// <returns></returns>
            public string StringReturnValue()
            {
                return "Hello!";
            }

            /// <summary>
            /// Returns hello with a given name.
            /// </summary>
            /// <param name="name">A name.</param>
            /// <returns></returns>
            public string SayHello(string name)
            {
                return string.Format("Hello {0}!", name);
            }

            /// <summary>
            /// Returns the template for the SayHelloWithAge
            /// </summary>
            /// <value>
            /// The template for the SayHelloWithAge
            /// </value>
            public string SayHelloWithAgeTemplate
            {
                get
                {
                    return "<h1>Templated Results</h1><p>{{model.result}}</p>";
                }
            }

            /// <summary>
            /// Returns the description of the SayHelloWithAgeDescription method
            /// </summary>
            /// <value>
            /// Description for the test method
            /// </value>
            public string SayHelloWithAgeDescription
            {
                get
                {
                    return "Simple test method that takes a name and an age and returns a result with a template.";
                }
            }

            /// <summary>
            /// Simple test method that takes a name and an age and returns a string.
            /// </summary>
            /// <param name="myName">A name.</param>
            /// <param name="myAge">An age.</param>
            /// <returns>A string with the given name and age.</returns>
            public string SayHelloWithAge(string myName, int myAge)
            {
                return string.Format("Hello {0}, you are {1} years old!", myName, myAge);
            }

            /// <summary>
            /// Returns a string with a name and an age using built-in attributes.
            /// </summary>
            /// <param name="myName">My name.</param>
            /// <param name="myAge">My age.</param>
            /// <returns>A templated string with the given name and age.</returns>
            [Template("<h1>Templated Results</h1><p>{{model.result}}</p>")]
            [Description("Simple test method that takes a name and an age and returns a result with a template.")]
            public string SayHelloWithAge2(string myName, int myAge)
            {
                return string.Format("Hello {0}, you are {1} years old!", myName, myAge);
            }
        }
    }
}