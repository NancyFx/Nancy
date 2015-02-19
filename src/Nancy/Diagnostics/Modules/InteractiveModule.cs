namespace Nancy.Diagnostics.Modules
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Linq;

    using Nancy.Helpers;

    public class InteractiveModule : DiagnosticModule
    {
        private readonly IInteractiveDiagnostics interactiveDiagnostics;

        public InteractiveModule(IInteractiveDiagnostics interactiveDiagnostics)
            :base ("/interactive")
        {
            this.interactiveDiagnostics = interactiveDiagnostics;

            Get["/"] = _ =>
            {
                return View["InteractiveDiagnostics"];
            };

            Get["/providers"] = _ =>
            {
                var providers = this.interactiveDiagnostics
                    .AvailableDiagnostics
                    .Select(p => new
                        {
                            p.Name,
                            p.Description,
                            Type = p.GetType().Name,
                            p.GetType().Namespace,
                            Assembly = p.GetType().Assembly.GetName().Name
                        })
                    .ToArray();

                return this.Response.AsJson(providers);
            };

            Get["/providers/{providerName}"] = ctx =>
            {
                var providerName =
                    HttpUtility.UrlDecode((string)ctx.providerName);

                var diagnostic =
                    this.interactiveDiagnostics.GetDiagnostic(providerName);

                if (diagnostic == null)
                {
                    return HttpStatusCode.NotFound;
                }

                var methods = diagnostic.Methods
                    .Select(m => new
                        {
                            m.MethodName,
                            ReturnType = m.ReturnType.ToString(),
                            m.Description,
                            Arguments = m.Arguments.Select(a => new
                            {
                                ArgumentName = a.Item1,
                                ArgumentType = a.Item2.ToString()
                            })
                        })
                    .ToArray();

                return this.Response.AsJson(methods);
            };

            Get["/providers/{providerName}/{methodName}"] = ctx =>
            {
                var providerName =
                    HttpUtility.UrlDecode((string)ctx.providerName);

                var methodName =
                    HttpUtility.UrlDecode((string)ctx.methodName);

                var method =
                    this.interactiveDiagnostics.GetMethod(providerName, methodName);

                if (method == null)
                {
                    return HttpStatusCode.NotFound;
                }

                object[] arguments =
                    GetArguments(method, this.Request.Query);

                return this.Response.AsJson(new { Result = this.interactiveDiagnostics.ExecuteDiagnostic(method, arguments) });
            };

            Get["/templates/{providerName}/{methodName}"] = ctx =>
            {
                var providerName =
                    HttpUtility.UrlDecode((string)ctx.providerName);

                var methodName =
                    HttpUtility.UrlDecode((string)ctx.methodName);

                var method =
                    this.interactiveDiagnostics.GetMethod(providerName, methodName);

                if (method == null)
                {
                    return HttpStatusCode.NotFound;
                }

                var template =
                    this.interactiveDiagnostics.GetTemplate(method);

                if (template == null)
                {
                    return HttpStatusCode.NotFound;
                }

                return template;
            };
        }

        private static object[] GetArguments(InteractiveDiagnosticMethod method, dynamic query)
        {
            var arguments = new List<object>();

            foreach (var argument in method.Arguments)
            {
                arguments.Add(ConvertArgument((string)query[argument.Item1].Value, argument.Item2));
            }

            return arguments.ToArray();
        }

        private static object ConvertArgument(string value, Type destinationType)
        {
            var converter =
                TypeDescriptor.GetConverter(destinationType);

            if (converter == null || !converter.CanConvertFrom(typeof(string)))
            {
                return null;
            }

            try
            {
                return converter.ConvertFrom(value);
            }
            catch (FormatException)
            {
                return null;
            }
        }
    }
}