namespace Nancy.Diagnostics
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;

    using Nancy.Routing;

    /// <summary>
    /// Handles interactive diagnostic instances.
    /// </summary>
    /// <seealso cref="Nancy.Diagnostics.IInteractiveDiagnostics" />
    public class InteractiveDiagnostics : IInteractiveDiagnostics
    {
        private readonly IDiagnosticsProvider[] providers;

        private const BindingFlags Flags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.FlattenHierarchy;

        /// <summary>
        /// Gets the list of available diagnostics.
        /// </summary>
        /// <value>
        /// The available diagnostics.
        /// </value>
        public IEnumerable<InteractiveDiagnostic> AvailableDiagnostics { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="InteractiveDiagnostics"/> class.
        /// </summary>
        /// <param name="providers">The providers.</param>
        public InteractiveDiagnostics(IEnumerable<IDiagnosticsProvider> providers)
        {
            var customProvidersAvailable = providers.Any(provider =>
            {
                Type providerType = provider.GetType();

                return providerType != typeof(TestingDiagnosticProvider) &
                       providerType != typeof(DefaultRouteCacheProvider);
            });

            if (customProvidersAvailable)
            {
                // Exclude only the TestingDiagnosticProvider
                this.providers = providers.Where(provider => provider.GetType() != typeof(TestingDiagnosticProvider)).ToArray();
            }
            else
            {
                this.providers = providers.ToArray();
            }

            this.BuildAvailableDiagnostics();
        }

        /// <summary>
        /// Executes the diagnostic.
        /// </summary>
        /// <param name="interactiveDiagnosticMethod">The interactive diagnostic method.</param>
        /// <param name="arguments">The arguments.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentException"></exception>
        public object ExecuteDiagnostic(InteractiveDiagnosticMethod interactiveDiagnosticMethod, object[] arguments)
        {
            var method = GetMethodInfo(interactiveDiagnosticMethod);

            if (method == null)
            {
                throw new ArgumentException(string.Format("Unable to locate method: {0}", interactiveDiagnosticMethod.MethodName));
            }

            return method.Invoke(interactiveDiagnosticMethod.ParentDiagnosticObject, arguments);
        }

        /// <summary>
        /// Gets the template for an interactive diagnostic method instance.
        /// </summary>
        /// <param name="interactiveDiagnosticMethod">The interactive diagnostic method.</param>
        /// <returns></returns>
        public string GetTemplate(InteractiveDiagnosticMethod interactiveDiagnosticMethod)
        {
            var diagObjectType = interactiveDiagnosticMethod.ParentDiagnosticObject.GetType();

            return GetTemplateFromProperty(interactiveDiagnosticMethod, diagObjectType) ??
                   GetTemplateFromAttribute(interactiveDiagnosticMethod);
        }

        /// <summary>
        /// Gets the diagnostic for a provider.
        /// </summary>
        /// <param name="providerName">Name of the provider.</param>
        /// <returns></returns>
        public InteractiveDiagnostic GetDiagnostic(string providerName)
        {
            return this.AvailableDiagnostics.FirstOrDefault(d => string.Equals(d.Name, providerName, StringComparison.OrdinalIgnoreCase));
        }

        /// <summary>
        /// Gets the method instance for a method name and provider.
        /// </summary>
        /// <param name="providerName">Name of the provider.</param>
        /// <param name="methodName">Name of the method.</param>
        /// <returns></returns>
        public InteractiveDiagnosticMethod GetMethod(string providerName, string methodName)
        {
            var diagnostic = this.GetDiagnostic(providerName);

            if (diagnostic == null)
            {
                return null;
            }

            return diagnostic.Methods.FirstOrDefault(m => string.Equals(m.MethodName, methodName, StringComparison.OrdinalIgnoreCase));
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
            var objectMethods = typeof(object).GetMethods().Select(x => x.Name).ToList();

            var methods = diagnosticsProvider.DiagnosticObject
                                             .GetType()
                                             .GetMethods(Flags)
                                             .Where(x => !objectMethods.Contains(x.Name))
                                             .Where(mi => !mi.IsSpecialName)
                                             .ToArray();

            var diagnosticMethods = new List<InteractiveDiagnosticMethod>(methods.Length);

            foreach (var methodInfo in methods)
            {
                diagnosticMethods.Add(new InteractiveDiagnosticMethod(
                                            diagnosticsProvider.DiagnosticObject,
                                            methodInfo.ReturnType,
                                            methodInfo.Name,
                                            this.GetArguments(methodInfo),
                                            this.GetDescription(diagnosticsProvider, methodInfo)));
            }

            return diagnosticMethods;
        }

        private string GetDescription(IDiagnosticsProvider diagnosticsProvider, MethodInfo methodInfo)
        {
            return GetDescriptionFromProperty(diagnosticsProvider, methodInfo) ??
                   GetDescriptionFromAttribute(diagnosticsProvider, methodInfo);
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

        private static string GetTemplateFromProperty(
            InteractiveDiagnosticMethod interactiveDiagnosticMethod, Type diagObjectType)
        {
            var propertyName = String.Format("{0}{1}", interactiveDiagnosticMethod.MethodName, "Template");
            var property = diagObjectType.GetProperty(propertyName);

            if (property == null)
            {
                return null;
            }

            return (string)property.GetValue(interactiveDiagnosticMethod.ParentDiagnosticObject, null);
        }

        private static string GetTemplateFromAttribute(InteractiveDiagnosticMethod interactiveDiagnosticMethod)
        {
            var method = GetMethodInfo(interactiveDiagnosticMethod);

            var attribute = (TemplateAttribute)method.GetCustomAttribute(typeof(TemplateAttribute));

            return attribute != null ? attribute.Template : null;
        }

        private static string GetDescriptionFromProperty(IDiagnosticsProvider diagnosticsProvider, MethodInfo methodInfo)
        {
            var propertyName = String.Format("{0}{1}", methodInfo.Name, "Description");
            var property = diagnosticsProvider.DiagnosticObject.GetType().GetProperty(propertyName);

            if (property == null)
            {
                return null;
            }

            return (string)property.GetValue(diagnosticsProvider.DiagnosticObject, null);
        }

        private static string GetDescriptionFromAttribute(IDiagnosticsProvider diagnosticsProvider, MethodInfo methodInfo)
        {
            var attribute = (DescriptionAttribute)methodInfo.GetCustomAttribute(typeof(DescriptionAttribute));

            return attribute != null ? attribute.Description : null;
        }

        private static MethodInfo GetMethodInfo(InteractiveDiagnosticMethod interactiveDiagnosticMethod)
        {
            var diagObjectType = interactiveDiagnosticMethod.ParentDiagnosticObject.GetType();

            var method = diagObjectType.GetMethod(interactiveDiagnosticMethod.MethodName, Flags);

            return method;
        }
    }
}