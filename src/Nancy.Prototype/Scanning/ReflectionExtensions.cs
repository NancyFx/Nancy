namespace Nancy.Prototype.Scanning
{
    using System.Reflection;

    internal static class ReflectionExtensions
    {
        public static bool IsAssignableTo(this TypeInfo sourceType, TypeInfo targetType)
        {
            Check.NotNull(sourceType, nameof(sourceType));
            Check.NotNull(targetType, nameof(targetType));

            return targetType.IsGenericTypeDefinition
                ? sourceType.IsAssignableToGenericTypeDefinition(targetType)
                : targetType.IsAssignableFrom(sourceType);
        }

        private static bool IsAssignableToGenericTypeDefinition(this TypeInfo sourceType, TypeInfo genericTypeDefinition)
        {
            if (sourceType.MatchesGenericTypeDefinition(genericTypeDefinition))
            {
                return true;
            }

            var interfaceTypes = sourceType.ImplementedInterfaces;

            foreach (var interfaceType in interfaceTypes)
            {
                var interfaceTypeInfo = interfaceType.GetTypeInfo();

                if (interfaceTypeInfo.MatchesGenericTypeDefinition(genericTypeDefinition))
                {
                    return true;
                }
            }

            var baseTypeInfo = sourceType.BaseType?.GetTypeInfo();

            if (baseTypeInfo == null)
            {
                return false;
            }

            return baseTypeInfo.IsAssignableToGenericTypeDefinition(genericTypeDefinition);
        }

        private static bool MatchesGenericTypeDefinition(this TypeInfo sourceType, TypeInfo genericTypeDefinition)
        {
            return sourceType.IsGenericType && sourceType.GetGenericTypeDefinition().GetTypeInfo() == genericTypeDefinition;
        }
    }
}
