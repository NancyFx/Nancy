namespace Nancy.Diagnostics
{
    public class TestingDiagnosticProvider : IDiagnosticsProvider
    {
        private readonly object diagObject;

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

        public class DiagObject
        {
            public void NoReturnValue()
            {

            }

            public string StringReturnValue()
            {
                return "Hello!";
            }

            public string SayHello(string name)
            {
                return string.Format("Hello {0}!", name);
            }

            public string SayHelloWithAgeTemplate
            {
                get
                {
                    return "<h1>Templated Results</h1><p>{{model.result}}</p>";
                }
            }

            public string SayHelloWithAgeDescription
            {
                get
                {
                    return "Simple test method that takes a name and an age and returns a result with a template.";
                }
            }

            public string SayHelloWithAge(string myName, int myAge)
            {
                return string.Format("Hello {0}, you are {1} years old!", myName, myAge);
            }

            [Template("<h1>Templated Results</h1><p>{{model.result}}</p>")]
            [Description("Simple test method that takes a name and an age and returns a result with a template.")]
            public string SayHelloWithAge2(string myName, int myAge)
            {
                return string.Format("Hello {0}, you are {1} years old!", myName, myAge);
            }
        }
    }
}