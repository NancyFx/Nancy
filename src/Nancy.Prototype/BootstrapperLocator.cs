namespace Nancy.Prototype
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using System.Text;
    using Nancy.Prototype.Scanning;

    public class BootstrapperLocator : IBootstrapperLocator
    {
        private readonly ITypeCatalog typeCatalog;

        public BootstrapperLocator(ITypeCatalog typeCatalog)
        {
            Check.NotNull(typeCatalog, nameof(typeCatalog));

            this.typeCatalog = typeCatalog;
        }

        public IBootstrapper GetBootstrapper()
        {
            var types = this.typeCatalog
                .GetTypesAssignableTo<IBootstrapper>(ScanningStrategies.ExcludeNancy)
                .ToArray();

            var type = GetBootstrapperType(types);

            try
            {
                return (IBootstrapper) Activator.CreateInstance(type);
            }
            catch (Exception ex)
            {
                throw new BootstrapperActivationException(
                    string.Format(Resources.Exception_BootstrapperActivation, type.FullName), ex);
            }
        }

        private static Type GetBootstrapperType(IList<Type> types)
        {
            if (types.Count == 0)
            {
                // TODO: Return default bootstrapper.
                throw new InvalidOperationException("Could not locate a bootstrapper implementation.");
            }

            if (types.Count == 1)
            {
                return types[0];
            }

            var candidateTypes = GetCandidateTypes(types);

            if (candidateTypes.Count == 1)
            {
                return candidateTypes[0];
            }

            var message = GenerateMultipleBootstrappersMessage(candidateTypes);

            throw new InvalidOperationException(message);
        }

        private static IList<Type> GetCandidateTypes(IList<Type> types)
        {
            var baseTypes = GetBaseTypes(types);

            var candidateTypes = new List<Type>();

            foreach (var type in types)
            {
                if (!baseTypes.Contains(type))
                {
                    candidateTypes.Add(type);
                }
            }

            return candidateTypes;
        }

        private static ISet<Type> GetBaseTypes(IEnumerable<Type> types)
        {
            var baseTypes = new HashSet<Type>();

            foreach (var type in types)
            {
                var baseType = type.GetTypeInfo().BaseType;

                if (baseType != null)
                {
                    baseTypes.Add(baseType);
                }
            }

            return baseTypes;
        }

        private static string GenerateMultipleBootstrappersMessage(IEnumerable<Type> types)
        {
            var builder = new StringBuilder();

            builder.AppendLine("Located multiple bootstrappers:");

            foreach (var type in types)
            {
                builder.AppendLine($" - {type.FullName}");
            }

            builder.AppendLine();
            builder.AppendLine("Either remove unused bootstrapper types or specify which type to use.");

            return builder.ToString();
        }
    }
}
