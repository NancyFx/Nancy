namespace Nancy.Diagnostics.Modules
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Linq;

    public class InteractiveModule : DiagnosticModule
    {
        private readonly IDiagnosticSessions sessionProvider;

        private readonly IInteractiveDiagnostics interactiveDiagnostics;

        public InteractiveModule(IDiagnosticSessions sessionProvider, IInteractiveDiagnostics interactiveDiagnostics)
            :base ("/interactive")
        {
            this.sessionProvider = sessionProvider;
            this.interactiveDiagnostics = interactiveDiagnostics;

            Get["/"] = _ => View["InteractiveDiagnostics"];

            Get["/providers"] = _ =>
                {
                    var providers = this.interactiveDiagnostics
                                        .AvailableDiagnostics
                                        .Select(p => new { p.Name, p.Description })
                                        .ToArray();

                    return Response.AsJson(providers);
                };

            Get["/providers/{providerName}"] = ctx =>
                {
                    var provider =
                        this.interactiveDiagnostics.AvailableDiagnostics.FirstOrDefault(
                            d => string.Equals(d.Name, ctx.providerName, StringComparison.OrdinalIgnoreCase));

                    if (provider == null)
                    {
                        return HttpStatusCode.NotFound;
                    }

                    var methods = provider.Methods
                                          .Select(m => new
                                              {
                                                  m.MethodName, 
                                                  ReturnType = m.ReturnType.ToString(), 
                                                  Arguments = m.Arguments.Select(a => new
                                                      {
                                                          ArgumentName = a.Item1, 
                                                          ArgumentType = a.Item2.ToString()
                                                      })
                                              })
                                          .ToArray();

                    return Response.AsJson(methods);
                };

            Get["/providers/{providerName}/{methodName}"] = ctx =>
                {
                    var provider =
                        this.interactiveDiagnostics.AvailableDiagnostics.FirstOrDefault(
                            d => string.Equals(d.Name, ctx.providerName, StringComparison.OrdinalIgnoreCase));

                    if (provider == null)
                    {
                        return HttpStatusCode.NotFound;
                    }

                    var method =
                        provider.Methods.FirstOrDefault(
                            m => string.Equals(m.MethodName, ctx.methodName, StringComparison.OrdinalIgnoreCase));

                    if (method == null)
                    {
                        return HttpStatusCode.NotFound;
                    }

                    object[] arguments = this.GetArguments(method, this.Request.Query);

                    return Response.AsJson(new { Result = this.interactiveDiagnostics.ExecuteDiagnostic(method, arguments) });
                };
        }

        private object[] GetArguments(InteractiveDiagnosticMethod method, dynamic query)
        {
            var arguments = new List<object>();

            foreach (var argument in method.Arguments)
            {
                arguments.Add(this.ConvertArgument((string)query[argument.Item1].Value, argument.Item2));
            }

            return arguments.ToArray();
        }

        private object ConvertArgument(string value, Type destinationType)
        {
            var converter = TypeDescriptor.GetConverter(destinationType);

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