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

        public object ExecuteDiagnostic(InteractiveDiagnosticMethod interactiveDiagnostic, object[] arguments)
        {
            var diagObjectType = interactiveDiagnostic.ParentDiagnosticObject.GetType();

            var method = diagObjectType.GetMethod(interactiveDiagnostic.MethodName, Flags);

            if (method == null)
            {
                throw new ArgumentException(string.Format("Unable to locate method: {0}", interactiveDiagnostic.MethodName));
            }

            return method.Invoke(interactiveDiagnostic.ParentDiagnosticObject, arguments);
        }

        private void BuildAvailableDiagnostics()
        {
            var diags = new List<InteractiveDiagnostic>(this.providers.Length);

            foreach (var diagnosticsProvider in this.providers)
            {
                diags.Add(new InteractiveDiagnostic
                    {
                        Name = diagnosticsProvider.Name,
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