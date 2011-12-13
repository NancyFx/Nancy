namespace Nancy.Diagnostics
{
    public class TestingDiagnosticProvider : IDiagnosticsProvider
    {
        private object diagObject;

        public TestingDiagnosticProvider()
        {
            this.diagObject = new DiagObject();
        }

        public string Name
        {
            get
            {
                return "Testing Diagnostic Provider";
            }
        }

        public string Description
        {
            get
            {
                return "Some testing methods that can be called to.. erm.. test things.";
            }
        }

        public object DiagnosticObject
        {
            get
            {
                return this.diagObject;
            }
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
        }
    }
}