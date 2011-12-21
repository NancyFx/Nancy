namespace Nancy.Diagnostics
{
    using System;
    using System.Collections.Generic;

    public class InteractiveDiagnosticMethod
    {
        public object ParentDiagnosticObject { get; private set; }

        public Type ReturnType { get; private set; }

        public string MethodName { get; private set; }

        public string Description { get; private set; }

        public IEnumerable<Tuple<string, Type>> Arguments { get; private set; }

        public InteractiveDiagnosticMethod(object parentDiagnostic, Type returnType, string methodName, IEnumerable<Tuple<string, Type>> arguments, string description)
        {
            this.ParentDiagnosticObject = parentDiagnostic;
            this.ReturnType = returnType;
            this.MethodName = methodName;
            this.Arguments = arguments;
            this.Description = description;
        }
    }
}