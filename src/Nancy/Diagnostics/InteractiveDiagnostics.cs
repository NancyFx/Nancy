namespace Nancy.Diagnostics
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;

    public class InteractiveDiagnostics : IInteractiveDiagnostics
    {
        private readonly IDiagnosticsProvider[] providers;

        private const BindingFlags Flags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.DeclaredOnly;

        public IEnumerable<InteractiveDiagnostic> AvailableDiagnostics { get; private set; }

        public InteractiveDiagnostics(IEnumerable<IDiagnosticsProvider> providers)
        {
            this.providers = providers.ToArray();

            this.BuildAvailableDiagnostics();
        }

        public object ExecuteDiagnostic(InteractiveDiagnosticMethod interactiveDiagnosticMethod, object[] arguments)
        {
            var diagObjectType = interactiveDiagnosticMethod.ParentDiagnosticObject.GetType();

            var method = diagObjectType.GetMethod(interactiveDiagnosticMethod.MethodName, Flags);

            if (method == null)
            {
                throw new ArgumentException(string.Format("Unable to locate method: {0}", interactiveDiagnosticMethod.MethodName));
            }

            return method.Invoke(interactiveDiagnosticMethod.ParentDiagnosticObject, arguments);
        }

        public string GetTemplate(InteractiveDiagnosticMethod interactiveDiagnosticMethod)
        {
            var diagObjectType = interactiveDiagnosticMethod.ParentDiagnosticObject.GetType();
            var propertyName = String.Format("{0}{1}", interactiveDiagnosticMethod.MethodName, "Template");
            var property = diagObjectType.GetProperty(propertyName);

            if (property == null)
            {
                return null;
            }

            return (string)property.GetValue(interactiveDiagnosticMethod.ParentDiagnosticObject, null);
        }

        private void BuildAvailableDiagnostics()
        {
            var diags = new List<InteractiveDiagnostic>(this.providers.Length);

            foreach (var diagnosticsProvider in this.providers)
            {
                diags.Add(new InteractiveDiagnostic
                    {
                        Name = diagnosticsProvider.Name,
                        Description = diagnosticsProvider.Description,
                        Methods = this.GetDiagnosticMethods(diagnosticsProvider)
                    });
            }

            this.AvailableDiagnostics = diags;
        }

        private IEnumerable<InteractiveDiagnosticMethod> GetDiagnosticMethods(IDiagnosticsProvider diagnosticsProvider)
        {
            var methods = diagnosticsProvider.DiagnosticObject.GetType().GetMethods(Flags);
            var diagnosticMethods = new List<InteractiveDiagnosticMethod>(methods.Length);

            foreach (var methodInfo in methods)
            {
                diagnosticMethods.Add(new InteractiveDiagnosticMethod(
                                            diagnosticsProvider.DiagnosticObject,
                                            methodInfo.ReturnType,
                                            methodInfo.Name,
                                            this.GetArguments(methodInfo)));
            }

            return diagnosticMethods;
        }

        private IEnumerable<Tuple<string, Type>> GetArguments(MethodInfo methodInfo)
        {
            var parameters = methodInfo.GetParameters();
            var arguments = new List<Tuple<string, Type>>(parameters.Length);

            foreach (var parameterInfo in parameters)
            {
                arguments.Add(Tuple.Create(parameterInfo.Name, parameterInfo.ParameterType));
            }

            return arguments;
        }
    }
}